using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using MandrilBot.News;
using System.Net;

namespace MandrilBot
{
    /// <summary>
    /// Class with definition of the Discord bot commands that can be used to interact with the bot from Discord.
    /// </summary>
    internal class BotCommands : BaseCommandModule
    {
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
                await aCommandContext.Channel.DeleteMessagesAsync(await aCommandContext.Channel.GetMessagesAsync());
                var lMemberList = await aCommandContext.Guild
                                         .GetAllMembersAsync()
                                         .ConfigureAwait(false);
                var lString = Utf8Json.JsonSerializer.ToJsonString(lMemberList.Select(x => $"{x.Username}#{x.Discriminator}").ToArray());
                await aCommandContext.Channel.SendMessageAsync(lString);
            }
            catch (BadRequestException)
            {
                await aCommandContext.Channel.SendMessageAsync("An error occurred, please notify the administrator.");
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
                await aCommandContext.Channel.SendMessageAsync("An error occurred, please notify the administrator.");
            }

        }
    }
}
