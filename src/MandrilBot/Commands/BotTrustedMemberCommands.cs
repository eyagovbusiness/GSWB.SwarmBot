using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MandrilBot.Commands
{
    /// <summary>
    /// Class with definition of the Discord bot commands that can be used to interact with the bot from Discord.
    /// </summary>
    internal class BotTrustedMemberCommands : BotAuthorizedCommands
    {
        public BotTrustedMemberCommands
            (IServiceScopeFactory aServiceScopeFactory, IConfiguration aConfiguration) 
                : base(aServiceScopeFactory, aConfiguration, "TrustedMemberRoleId")
        {
        }


        [Command("start-service")]
        public async Task StartServiceBotCommand(CommandContext aCommandContext)
        {
            /*var lEventCategoryId = aCommandContext.Channel.Parent.Id;*/ //With this we can go to the web DB and read the event associated with this category.
                                                                          //Next step would be to read to which channel the user who sent the command is assignes in this event and move him to that channel.
            try
            {
                await aCommandContext.Member
                                     .PlaceInAsync(aCommandContext.Channel.Parent.Children
                                        .FirstOrDefault(x => x.Type == DSharpPlus.ChannelType.Voice))
                                     .ConfigureAwait(false);
            }
            catch (BadRequestException)
            {
                await aCommandContext.Channel.SendMessageAsync("Please, be connected to any voice channel in this server before reporting for service :)");
            }

        }

        [Command("members")]
        public async Task GetMemberList(CommandContext aCommandContext)
        {
            try
            {
                //await aCommandContext.Channel.DeleteMessagesAsync(await aCommandContext.Channel.GetMessagesAsync());
                var lMemberList = await aCommandContext.Guild
                                         .GetAllMembersAsync()
                                         .ConfigureAwait(false);
                var lString = Utf8Json.JsonSerializer.ToJsonString(lMemberList.Select(x => $"{x.Username}#{x.Discriminator}").ToArray());
                await aCommandContext.Channel.SendMessageAsync(lString);
            }
            catch (BadRequestException)
            {
                await aCommandContext.Channel.SendMessageAsync("Something went wrong!");
            }

        }

        [Command("clear")]
        public async Task Clear(CommandContext aCommandContext)
        {
            try
            {
                await aCommandContext.Channel.DeleteMessagesAsync(await aCommandContext.Channel.GetMessagesAsync());
            }
            catch (BadRequestException)
            {
                await aCommandContext.Channel.SendMessageAsync("Something went wrong!");
            }

        }

        [Command("moveall")]
        [Description("Move all members with a specified role or higher in the hierarchy to a specified channel.")]
        public async Task MoveAll(CommandContext ctx,
                          [Description("Role to start filtering members by hierarchy.")] DiscordRole baseRole,
                          [Description("Channel to move members to.")] DiscordChannel targetChannel,
                          [Description("Whether to move all members regardless of their current channel.")] bool global = false)
        {
            if (!await IsMemberAuthorized(ctx.Member))
                return;

            if (targetChannel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Please specify a valid voice channel!");
                return;
            }

            // Fetch all guild members
            var allMembers = await ctx.Guild.GetAllMembersAsync();

            // Determine base role position in the role hierarchy
            int baseRolePosition = baseRole.Position;

            // Filter members by the role or higher in the hierarchy
            var membersToMove = allMembers.Where(m => m.Roles.Any(r => r.Position >= baseRolePosition) && m.VoiceState?.Channel != null).ToList();

            // If not global, filter members who are in the same channel as the command caller
            if (!global)
            {
                var callerChannel = ctx.Member.VoiceState?.Channel;
                if (callerChannel == null)
                {
                    await ctx.RespondAsync("You must be in a voice channel to use the non-global move!");
                    return;
                }
                membersToMove = membersToMove.Where(m => m.VoiceState?.Channel == callerChannel).ToList();
            }

            // Move each member to the target channel
            foreach (var member in membersToMove)
            {
                try
                {
                    await member.ModifyAsync(m => m.VoiceChannel = targetChannel);
                }
                catch (Exception ex)
                {
                    await ctx.RespondAsync($"Failed to move {member.Username}: {ex.Message}");
                }
            }

            await ctx.RespondAsync($"Moved members with the role `{baseRole.Name}` or higher to `{targetChannel.Name}`.");
        }

    }
}
