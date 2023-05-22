using AngleSharp;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using MandrilBot.BackgroundServices.NewMemberManager;

namespace MandrilBot.Commands
{
    /// <summary>
    /// Class with definition of the Discord bot commands that can be used to interact with the bot from Discord only allowed to member with the defined Admin role.
    /// </summary>
    internal class BotAdminCommands : BaseCommandModule
    {
        private readonly ulong _botAdminRoleId;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        //Avaliable if needed to inject IMandrilDiscordBot
        public BotAdminCommands(IServiceScopeFactory aServiceScopeFactory, IConfiguration aConfiguration)
        {
            _serviceScopeFactory = aServiceScopeFactory;
            _botAdminRoleId = aConfiguration.GetValue<ulong>("BotAdminRoleId");
        }

        private bool HasAdminRole(DiscordMember aDiscordMember)
            => aDiscordMember.Roles.Any(x => x.Id == _botAdminRoleId);

        [Command("get-newjoined")]
        public async Task GetNewJoined(CommandContext aCommandContext)
        {
            try
            {
                if (!HasAdminRole(aCommandContext.Member))
                    return;

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var lNewMemberManagerService = scope.ServiceProvider.GetRequiredService<INewMemberManagementService>();
                    var lNewMemberList = await lNewMemberManagerService.GetNewDiscordMemberList((DiscordMember member) => true);
                    var lMessageContent = string.Join($"{Environment.NewLine}", lNewMemberList.Select(member => $"{member.Nickname ?? member.DisplayName} joined {member.JoinedAt}"));
                    await aCommandContext.Channel.SendMessageAsync(lMessageContent);
                }
            }
            catch (BadRequestException)
            {
                await aCommandContext.Channel.SendMessageAsync("Something went wrong!");
            }

        }
    }
}
