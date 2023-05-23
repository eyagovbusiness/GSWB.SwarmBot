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
        private readonly IMandrilDiscordBot _mandrilDiscordBot;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ulong _botAdminRoleId;

        public BotAdminCommands(IMandrilDiscordBot aMandrilDiscordBot, IServiceScopeFactory aServiceScopeFactory, IConfiguration aConfiguration)
        {
            _mandrilDiscordBot = aMandrilDiscordBot;
            _serviceScopeFactory = aServiceScopeFactory;
            _botAdminRoleId = aConfiguration.GetValue<ulong>("BotAdminRoleId");
        }

        private bool HasAdminRole(DiscordMember aDiscordMember)
            => aDiscordMember.Roles.Any(x => x.Id == _botAdminRoleId);

        /// <summary>
        /// This command makes the bot to reply a message with the list of the current members with the NoMediaRole 
        /// and the time when they joined the guild.
        /// </summary>
        /// <param name="aCommandContext"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This command disables temporarily the auto-ban of new joined bots.
        /// </summary>
        /// <param name="aCommandContext"></param>
        /// <returns></returns>
        [Command("open-bots")]
        public async Task OpenBots(CommandContext aCommandContext)
        {
            try
            {
                if (!HasAdminRole(aCommandContext.Member))
                    return;

                await aCommandContext.Channel.SendMessageAsync("New bots will be allowed to join the server during the next 5 minutes...");
                if(await _mandrilDiscordBot.AllowTemporarilyJoinNewBots(5))
                    await aCommandContext.Channel.SendMessageAsync("New bots are no longer allowed join the server.");
                else 
                    await aCommandContext.Channel.SendMessageAsync("New bots were already allowed join the server at this time.");

            }
            catch (BadRequestException)
            {
                await aCommandContext.Channel.SendMessageAsync("Something went wrong!");
            }

        }
    }
}
