using DSharpPlus.EventArgs;
using DSharpPlus;
using Mandril.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TGF.CA.Infrastructure.Communication.Publisher.Integration;
using Mandril.Infrastructure.Communication.Messages;
using Mandril.Application.Mapping;
using Mandril.Application.DTOs;

namespace Mandril.Infrastructure.Communication.MessageProducer
{
    public class MandrilMessageProducer : IHostedService
    {
        private readonly IMandrilDiscordBot _mandrilDiscordBot;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public MandrilMessageProducer(IMandrilDiscordBot aMandrilBot, IServiceScopeFactory aServiceScopeFactory)
        {
            _mandrilDiscordBot = aMandrilBot;
            _serviceScopeFactory = aServiceScopeFactory;
        }

        #region IHostedService
        public Task StartAsync(CancellationToken aCancellationToken)
        {
            _mandrilDiscordBot.GuildMemberUpdated += MandrilMessageProducer_GuildMemberUpdated;
            _mandrilDiscordBot.GuildRoleCreated += MandrilDiscordBot_GuildRoleCreated;
            _mandrilDiscordBot.GuildRoleDeleted += MandrilDiscordBot_GuildRoleDeleted;
            _mandrilDiscordBot.GuildRoleUpdated += MandrilDiscordBot_GuildRoleUpdated;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken aCancellationToken)
        {
            _mandrilDiscordBot.GuildMemberUpdated -= MandrilMessageProducer_GuildMemberUpdated;
            _mandrilDiscordBot.GuildRoleCreated -= MandrilDiscordBot_GuildRoleCreated;
            _mandrilDiscordBot.GuildRoleDeleted -= MandrilDiscordBot_GuildRoleDeleted;
            _mandrilDiscordBot.GuildRoleUpdated -= MandrilDiscordBot_GuildRoleUpdated;
            return Task.CompletedTask;
        }
        #endregion

        private async Task MandrilMessageProducer_GuildMemberUpdated(DiscordClient sender, GuildMemberUpdateEventArgs args)
        {
            var lAddedRoles = args.RolesAfter.Except(args.RolesBefore).ToList();
            var lRemovedRoles = args.RolesBefore.Except(args.RolesAfter).ToList();

            if (lAddedRoles.Any())
                await SendMessage(new MemberRoleAddedDTO(args.MemberAfter.Id, lAddedRoles.Select(role => role.ToDto()).ToArray()), aRoutingKey: "mandril.members.sync");
            if (lRemovedRoles.Any())
                await SendMessage(new MemberRoleRevokedDTO(args.MemberAfter.Id, lRemovedRoles.Select(role => role.ToDto()).ToArray()), aRoutingKey: "mandril.members.sync");
        }

        private async Task MandrilDiscordBot_GuildRoleUpdated(DiscordClient sender, GuildRoleUpdateEventArgs args)
            => await SendMessage(new RoleUpdatedDTO(new DiscordRoleDTO(args.RoleAfter.Id, args.RoleAfter.Name, (byte)args.RoleAfter.Position)), aRoutingKey: "mandril.roles.sync");

        private async Task MandrilDiscordBot_GuildRoleDeleted(DiscordClient sender, GuildRoleDeleteEventArgs args)
            => await SendMessage(new RoleDeletedDTO(args.Role.Id), aRoutingKey: "mandril.roles.sync");

        private async Task MandrilDiscordBot_GuildRoleCreated(DiscordClient sender, GuildRoleCreateEventArgs args)
            => await SendMessage(new RoleCreatedDTO(new DiscordRoleDTO(args.Role.Id, args.Role.Name, (byte)args.Role.Position)), aRoutingKey: "mandril.roles.sync");

        private async Task SendMessage(object aMessage, string? aRoutingKey = default)
        {
            using var lScope = _serviceScopeFactory.CreateScope();
            var lIntegrationMessagePublisher = lScope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>();
            await lIntegrationMessagePublisher.Publish(aMessage, routingKey: aRoutingKey);
        }

    }
}
