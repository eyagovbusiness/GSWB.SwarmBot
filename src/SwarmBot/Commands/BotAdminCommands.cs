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
                if (!await IsMemberAuthorized(aCommandContext.Member))
                    return;

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var lNewMemberManagerService = scope.ServiceProvider.GetRequiredService<INewMemberManagementService>();
                    var lNoMediaDays = lNewMemberManagerService.GetNoMediaDays();
                    var lNewMemberList = await lNewMemberManagerService.GetNewDiscordMemberList((DiscordMember member) => true);
                    string lJoinString = $"{Environment.NewLine} {Environment.NewLine}";//jump line and leave one empty line below each entry
                    var lMessageContent = string.Join(lJoinString,
                        lNewMemberList
                        .OrderBy(member => member.JoinedAt)
                        .Select((member, index) => $"{++index} - <@{member.Id}> joined {GetPastTimeSince(member.JoinedAt)}, created {GetPastTimeSince(member.CreationTimestamp)}. {GetNewMemberEmojiInfo(member, lNoMediaDays)}"));
                    await aCommandContext.Channel.SendMessageAsync(new DiscordMessageBuilder()
                    {
                        Embed = new DiscordEmbedBuilder()
                        {
                            Title = $"New members list",
                            Description = lMessageContent,
                            Color = DiscordColor.Goldenrod
                        }
                    });

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
                if (!await IsMemberAuthorized(aCommandContext.Member))
                    return;

                await aCommandContext.Channel.SendMessageAsync("New bots will be allowed to join the server during the next 5 minutes...");
                if (await _SwarmBotDiscordBot.AllowTemporarilyJoinNewBots(5))
                    await aCommandContext.Channel.SendMessageAsync("New bots are no longer allowed join the server.");
                else
                    await aCommandContext.Channel.SendMessageAsync("New bots were already allowed join the server at this time.");

            }
            catch (BadRequestException)
            {
                await aCommandContext.Channel.SendMessageAsync("Something went wrong!");
            }

        }

        #region Helpers
        /// <summary>
        /// Get a fancy string representing the time that passed since the provided <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="aDateTimeOffset"></param>
        /// <returns></returns>
        private string GetPastTimeSince(DateTimeOffset aDateTimeOffset)
        {
            TimeSpan lTimePast = DateTimeOffset.Now - aDateTimeOffset;

            string lResult = lTimePast.TotalDays switch
            {
                < 1 => $"{(int)lTimePast.TotalHours} hours ago",
                < 2 => "yesterday",
                < 30 => $"{(int)lTimePast.TotalDays} days ago",
                < 365 => $"{(int)(lTimePast.TotalDays / 30)} months, {(int)(lTimePast.TotalDays % 30)} days ago",
                _ => $"{(int)(lTimePast.TotalDays / 365)} years, {(int)((lTimePast.TotalDays % 365) / 30)} months, {(int)((lTimePast.TotalDays % 365) % 30)} days ago"
            };

            return lResult;
        }

        /// <summary>
        /// Gets an string representing information about the new member status with emojis.
        /// </summary>
        /// <param name="aDiscordMember"></param>
        /// <param name="aNoMediaDays"></param>
        /// <returns></returns>
        private string GetNewMemberEmojiInfo(DiscordMember aDiscordMember, int aNoMediaDays)
        {
            string lRes = string.Empty;
            var lTimeNow = DateTimeOffset.Now;
            //if the new member account was created only two weeks ago or less.
            if ((lTimeNow - aDiscordMember.CreationTimestamp).TotalDays <= 14)
                lRes += ":warning:";
            //if the new member will accquire soon the MediaRole
            var lTotalJoinedDays = (lTimeNow - aDiscordMember.JoinedAt).TotalDays;
            if (lTotalJoinedDays >= Convert.ToInt32(aNoMediaDays * 0.8))
            {
                var lDays = aNoMediaDays - lTotalJoinedDays;
                lRes += ":arrow_double_up:";
                if (lDays < 1)
                {
                    var lHours = (int)(lDays * 24);
                    lRes += lHours < 1
                        ? "(less than 1 hour)"
                        : $"({lHours} hours)";
                }
                else
                    lRes += (lDays >= 1 && lDays < 2)
                        ? $"(1 day)"
                        : lRes += $"({(int)lDays} days)";
            }
            return lRes;
        }

        #endregion

    }
}
