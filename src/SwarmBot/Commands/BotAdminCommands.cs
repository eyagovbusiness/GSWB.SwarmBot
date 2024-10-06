using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using SwarmBot.Application;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SwarmBot.Commands
{
    /// <summary>
    /// Class with definition of the Discord bot commands that can be used to interact with the bot from Discord only allowed to member with the defined Admin role.
    /// </summary>
    internal class BotAdminCommands : BotAuthorizedCommands
    {
        private readonly ISwarmBotDiscordBot _SwarmBotDiscordBot;

        public BotAdminCommands
            (ISwarmBotDiscordBot aSwarmBotDiscordBot, IServiceScopeFactory aServiceScopeFactory, IConfiguration aConfiguration)
                : base(aServiceScopeFactory, aConfiguration, "BotAdminRoleId")
        {
            _SwarmBotDiscordBot = aSwarmBotDiscordBot;
        }

        [Command("hello")]
        [Description("Just says Hello")]
        public async Task MoveFromRole(CommandContext aContext)
        { 
            await aContext.RespondAsync("Hello");
        }

    }
}
