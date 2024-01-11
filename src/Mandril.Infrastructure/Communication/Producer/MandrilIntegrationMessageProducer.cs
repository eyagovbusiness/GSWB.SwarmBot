using DSharpPlus;
using DSharpPlus.EventArgs;
using Mandril.Application;
using Mandril.Application.DTOs;
using Mandril.Application.Mapping;
using Mandril.Infrastructure.Communication.Messages;
using MandrilBot.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TGF.CA.Infrastructure.Communication.Publisher.Integration;

namespace Mandril.Infrastructure.Communication.MessageProducer
{
    public class MandrilIntegrationMessageProducer(IMandrilDiscordBot aMandrilBot, IServiceScopeFactory aServiceScopeFactory) : IHostedService
    {
        #region IHostedService
        public Task StartAsync(CancellationToken aCancellationToken)
        {
            aMandrilBot.GuildMemberUpdated += MandrilMessageProducer_GuildMemberUpdated;
            aMandrilBot.GuildRoleCreated += MandrilDiscordBot_GuildRoleCreated;
            aMandrilBot.GuildRoleDeleted += MandrilDiscordBot_GuildRoleDeleted;
            aMandrilBot.GuildRoleUpdated += MandrilDiscordBot_GuildRoleUpdated;
            aMandrilBot.GuildBanAdded += MandrilDiscordBot_GuildBanAdded;
            aMandrilBot.GuildBanRemoved += MandrilDiscordBot_GuildBanRemoved;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken aCancellationToken)
        {
            aMandrilBot.GuildMemberUpdated -= MandrilMessageProducer_GuildMemberUpdated;
            aMandrilBot.GuildRoleCreated -= MandrilDiscordBot_GuildRoleCreated;
            aMandrilBot.GuildRoleDeleted -= MandrilDiscordBot_GuildRoleDeleted;
            aMandrilBot.GuildRoleUpdated -= MandrilDiscordBot_GuildRoleUpdated;
            aMandrilBot.GuildBanAdded -= MandrilDiscordBot_GuildBanAdded;
            aMandrilBot.GuildBanRemoved -= MandrilDiscordBot_GuildBanRemoved;
            return Task.CompletedTask;
        }
        #endregion

        #region Event Handlers
        private async Task MandrilMessageProducer_GuildMemberUpdated(DiscordClient sender, GuildMemberUpdateEventArgs args)
        {
            await SendIfGuildMemberRoleUpdate(args);
            await SendIfGuildMemberDisplayNameUpdate(args);
            await SendIfGuildMemberAvatarUpdate(args);
        }

        private async Task MandrilDiscordBot_GuildRoleUpdated(DiscordClient sender, GuildRoleUpdateEventArgs args)
            => await SendMessage(new RoleUpdated(new DiscordRoleDTO(args.RoleAfter.Id.ToString(), args.RoleAfter.Name, (byte)args.RoleAfter.Position)), aRoutingKey: "mandril.roles.sync");

        private async Task MandrilDiscordBot_GuildRoleDeleted(DiscordClient sender, GuildRoleDeleteEventArgs args)
            => await SendMessage(new RoleDeleted(args.Role.Id.ToString()), aRoutingKey: "mandril.roles.sync");

        private async Task MandrilDiscordBot_GuildRoleCreated(DiscordClient sender, GuildRoleCreateEventArgs args)
            => await SendMessage(new RoleCreated(new DiscordRoleDTO(args.Role.Id.ToString(), args.Role.Name, (byte)args.Role.Position)), aRoutingKey: "mandril.roles.sync");

        private async Task MandrilDiscordBot_GuildBanRemoved(DiscordClient sender, GuildBanRemoveEventArgs args)
            => await SendMessage(new MemberBanUpdated(args.Member.Id.ToString(), args.Guild.Id.ToString(), true), aRoutingKey: "mandril.members.sync");

        private async Task MandrilDiscordBot_GuildBanAdded(DiscordClient sender, GuildBanAddEventArgs args)
            => await SendMessage(new MemberBanUpdated(args.Member.Id.ToString(), args.Guild.Id.ToString(), false), aRoutingKey: "mandril.members.sync");

        #endregion

        #region Helpers
        private async Task SendMessage(object aMessageContent, string? aRoutingKey = default)
        {
            using var lScope = aServiceScopeFactory.CreateScope();
            var lIntegrationMessagePublisher = lScope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>();
            await lIntegrationMessagePublisher.Publish(aMessageContent, routingKey: aRoutingKey);
        }

        private async Task SendIfGuildMemberRoleUpdate(GuildMemberUpdateEventArgs aGuildMemberUpdateEventArgs)
        {
            var lAddedRoles = aGuildMemberUpdateEventArgs.RolesAfter.Except(aGuildMemberUpdateEventArgs.RolesBefore).ToList();
            var lRemovedRoles = aGuildMemberUpdateEventArgs.RolesBefore.Except(aGuildMemberUpdateEventArgs.RolesAfter).ToList();

            if (lAddedRoles.Count != 0)
                await SendMessage(new MemberRoleAssigned(aGuildMemberUpdateEventArgs.MemberAfter.Id.ToString(), lAddedRoles.Select(role => role.ToDto()).ToArray()), aRoutingKey: "mandril.members.sync");
            if (lRemovedRoles.Count != 0)
                await SendMessage(new MemberRoleRevoked(aGuildMemberUpdateEventArgs.MemberAfter.Id.ToString(), lRemovedRoles.Select(role => role.ToDto()).ToArray()), aRoutingKey: "mandril.members.sync");
        }

        private async Task SendIfGuildMemberDisplayNameUpdate(GuildMemberUpdateEventArgs aGuildMemberUpdateEventArgs)
        {
            //TO-DO: GSWB-46
            if (aGuildMemberUpdateEventArgs.NicknameAfter != aGuildMemberUpdateEventArgs.NicknameBefore
                || aGuildMemberUpdateEventArgs.UsernameAfter != aGuildMemberUpdateEventArgs.UsernameBefore)
                await SendMessage(new MemberRenamed(aGuildMemberUpdateEventArgs.MemberAfter.Id.ToString(), aGuildMemberUpdateEventArgs.MemberAfter.DisplayName), aRoutingKey: "mandril.members.sync");
        }

        private async Task SendIfGuildMemberAvatarUpdate(GuildMemberUpdateEventArgs aGuildMemberUpdateEventArgs)
        {
            //TO-DO: GSWB-28
            if (aGuildMemberUpdateEventArgs.GuildAvatarHashAfter != aGuildMemberUpdateEventArgs.GuildAvatarHashBefore
                || aGuildMemberUpdateEventArgs.AvatarHashAfter != aGuildMemberUpdateEventArgs.AvatarHashBefore)
                await SendMessage(new MemberAvatarUpdated(aGuildMemberUpdateEventArgs.MemberAfter.Id.ToString(), aGuildMemberUpdateEventArgs.MemberAfter.GetGuildAvatarUrlOrDefault()), aRoutingKey: "mandril.members.sync");
        }

        #endregion

    }
}
