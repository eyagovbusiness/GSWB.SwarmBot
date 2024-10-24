using DSharpPlus;
using DSharpPlus.EventArgs;
using SwarmBot.Application;
using Common.Application.DTOs.Discord;
using SwarmBot.Application.Mapping;
using Common.Application.Contracts.Communication.Messages.Discord;
using SwarmBot.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Common.Application.Contracts.Communication;
using TGF.CA.Application.Contracts.Communication;

namespace SwarmBot.Infrastructure.Communication.MessageProducer
{
    public class SwarmBotIntegrationMessageProducer(ISwarmBotDiscordBot aSwarmBot, IServiceScopeFactory aServiceScopeFactory) : IHostedService
    {
        #region IHostedService
        public Task StartAsync(CancellationToken aCancellationToken)
        {
            aSwarmBot.GuildMemberUpdated += SwarmBotMessageProducer_GuildMemberUpdated;
            aSwarmBot.GuildRoleCreated += SwarmBotDiscordBot_GuildRoleCreated;
            aSwarmBot.GuildRoleDeleted += SwarmBotDiscordBot_GuildRoleDeleted;
            aSwarmBot.GuildRoleUpdated += SwarmBotDiscordBot_GuildRoleUpdated;
            aSwarmBot.GuildBanAdded += SwarmBotDiscordBot_GuildBanAdded;
            aSwarmBot.GuildBanRemoved += SwarmBotDiscordBot_GuildBanRemoved;
            aSwarmBot.GuildAdded += SwarmBotDiscordBot_GuildAdded;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken aCancellationToken)
        {
            aSwarmBot.GuildMemberUpdated -= SwarmBotMessageProducer_GuildMemberUpdated;
            aSwarmBot.GuildRoleCreated -= SwarmBotDiscordBot_GuildRoleCreated;
            aSwarmBot.GuildRoleDeleted -= SwarmBotDiscordBot_GuildRoleDeleted;
            aSwarmBot.GuildRoleUpdated -= SwarmBotDiscordBot_GuildRoleUpdated;
            aSwarmBot.GuildBanAdded -= SwarmBotDiscordBot_GuildBanAdded;
            aSwarmBot.GuildBanRemoved -= SwarmBotDiscordBot_GuildBanRemoved;
            return Task.CompletedTask;
        }
        #endregion

        #region Event Handlers
        private async Task SwarmBotMessageProducer_GuildMemberUpdated(DiscordClient sender, GuildMemberUpdateEventArgs args)
        {
            await SendIfGuildMemberRoleUpdate(args);
            await SendIfGuildMemberDisplayNameUpdate(args);
            await SendIfGuildMemberAvatarUpdate(args);
        }

        private async Task SwarmBotDiscordBot_GuildRoleUpdated(DiscordClient sender, GuildRoleUpdateEventArgs args)
            => await SendMessage(new RoleUpdated(args.Guild.Id.ToString(), new DiscordRoleDTO(args.Guild.Id.ToString(), args.RoleAfter.Id.ToString(), args.RoleAfter.Name, (byte)args.RoleAfter.Position)), aRoutingKey: RoutingKeys.SwarmBot.SwarmBot_Roles_sync);

        private async Task SwarmBotDiscordBot_GuildRoleDeleted(DiscordClient sender, GuildRoleDeleteEventArgs args)
            => await SendMessage(new RoleDeleted(args.Guild.Id.ToString(), args.Role.Id.ToString()), aRoutingKey: RoutingKeys.SwarmBot.SwarmBot_Roles_sync);

        private async Task SwarmBotDiscordBot_GuildRoleCreated(DiscordClient sender, GuildRoleCreateEventArgs args)
            => await SendMessage(new RoleCreated(args.Guild.Id.ToString(),new DiscordRoleDTO(args.Guild.Id.ToString(), args.Role.Id.ToString(), args.Role.Name, (byte)args.Role.Position)), aRoutingKey: RoutingKeys.SwarmBot.SwarmBot_Roles_sync);

        private async Task SwarmBotDiscordBot_GuildBanRemoved(DiscordClient sender, GuildBanRemoveEventArgs args)
            => await SendMessage(new MemberBanUpdated(args.Member.Id.ToString(), args.Guild.Id.ToString(), true), aRoutingKey: RoutingKeys.SwarmBot.SwarmBot_Members_sync);

        private async Task SwarmBotDiscordBot_GuildBanAdded(DiscordClient sender, GuildBanAddEventArgs args)
            => await SendMessage(new MemberBanUpdated(args.Member.Id.ToString(), args.Guild.Id.ToString(), false), aRoutingKey: RoutingKeys.SwarmBot.SwarmBot_Members_sync);

        private async Task SwarmBotDiscordBot_GuildAdded(DiscordClient sender, GuildCreateEventArgs args)
            => await SendMessage(new GuildAdded(args.Guild.Id.ToString(), args.Guild.Name, args.Guild.IconUrl), aRoutingKey: RoutingKeys.Guilds.Guilds_sync);

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
                await SendMessage(new MemberRoleAssigned(aGuildMemberUpdateEventArgs.MemberAfter.Id.ToString(), aGuildMemberUpdateEventArgs.MemberAfter.Guild.Id.ToString(), lAddedRoles.Select(role => role.ToDto(aGuildMemberUpdateEventArgs.Guild.Id)).ToArray()), aRoutingKey: RoutingKeys.SwarmBot.SwarmBot_Members_sync);
            if (lRemovedRoles.Count != 0)
                await SendMessage(new MemberRoleRevoked(aGuildMemberUpdateEventArgs.MemberAfter.Id.ToString(), aGuildMemberUpdateEventArgs.MemberAfter.Guild.Id.ToString(), lRemovedRoles.Select(role => role.ToDto(aGuildMemberUpdateEventArgs.Guild.Id)).ToArray()), aRoutingKey: RoutingKeys.SwarmBot.SwarmBot_Members_sync);
        }

        private async Task SendIfGuildMemberDisplayNameUpdate(GuildMemberUpdateEventArgs aGuildMemberUpdateEventArgs)
        {
            //TO-DO: GSWB-46
            if (aGuildMemberUpdateEventArgs.NicknameAfter != aGuildMemberUpdateEventArgs.NicknameBefore
                || aGuildMemberUpdateEventArgs.UsernameAfter != aGuildMemberUpdateEventArgs.UsernameBefore)
                await SendMessage(new MemberRenamed(aGuildMemberUpdateEventArgs.MemberAfter.Id.ToString(), aGuildMemberUpdateEventArgs.MemberAfter.Guild.Id.ToString(), aGuildMemberUpdateEventArgs.MemberAfter.DisplayName), aRoutingKey: RoutingKeys.SwarmBot.SwarmBot_Members_sync);
        }

        private async Task SendIfGuildMemberAvatarUpdate(GuildMemberUpdateEventArgs aGuildMemberUpdateEventArgs)
        {
            //TO-DO: GSWB-28
            if (aGuildMemberUpdateEventArgs.GuildAvatarHashAfter != aGuildMemberUpdateEventArgs.GuildAvatarHashBefore
                || aGuildMemberUpdateEventArgs.AvatarHashAfter != aGuildMemberUpdateEventArgs.AvatarHashBefore)
                await SendMessage(new MemberAvatarUpdated(aGuildMemberUpdateEventArgs.MemberAfter.Id.ToString(), aGuildMemberUpdateEventArgs.MemberAfter.Guild.Id.ToString(), aGuildMemberUpdateEventArgs.MemberAfter.GetGuildAvatarUrlOrDefault()), aRoutingKey: RoutingKeys.SwarmBot.SwarmBot_Members_sync);
        }

        #endregion

    }
}
