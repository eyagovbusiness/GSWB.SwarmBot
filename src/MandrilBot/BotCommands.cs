using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
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

        [Command("getlastnews")]
        public async Task GetLastNews(CommandContext aCommandContext)
        {
            try
            {
                var lBaseAddress = "https://robertsspaceindustries.com";
                HttpClient _httpClient = new(){ BaseAddress = new Uri(lBaseAddress) };
                var lResponse = await _httpClient.GetAsync("community/devtracker");
                var lStringResponse = await lResponse.Content.ReadAsStringAsync();

                var lParser = new HtmlParser();
                var lHTMLdocument = lParser.ParseDocument(lStringResponse);

                var lElementList = lHTMLdocument.QuerySelector("div.devtracker-list.js-devtracker-list")?.QuerySelector(".devtracker-list");
                var lDuctionaryData = lElementList.Children.Select(y => y.ToDictionary());

                string lSourceLink = lBaseAddress + lDuctionaryData.First()["PathName"];
                /// [1] autor 
                /// [3] date 
                /// [5] title 
                /// [6] short desc
                var lContent = lDuctionaryData.First()["TextContent"].Split('\n')
               .Where(x => !string.IsNullOrWhiteSpace(x))
               .ToArray();

                string lAutor = lContent[1].Trim(); string lDate = lContent[3].Trim(); string lTitle = lContent[5].Trim(); string lDesc = lContent[6].Trim();

                string lAuthorUrl = $"citizens/{lAutor}";

                await aCommandContext.Channel.SendMessageAsync(new DiscordMessageBuilder()
                {
                    Embed = new DiscordEmbedBuilder()
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor()
                        {
                            Name = lAutor,
                            Url = $"{lBaseAddress}/{lAuthorUrl}",
                            IconUrl = $"{lBaseAddress}{await GetCitizenImage(lAuthorUrl)}",
                        },
                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Height = 10, Width = 10, Url = "w" },
                        Title = lTitle,
                        Description = lDesc,  
                        Url = lSourceLink,
                        Color = DiscordColor.CornflowerBlue
                    }
                });

            }
            catch (BadRequestException)
            {
                await aCommandContext.Channel.SendMessageAsync("An error occurred, please notify the administrator.");
            }

        }

        private async Task<string> GetCitizenImage(string aAuthorUrl)
        {
            HttpClient _httpClient = new() { BaseAddress = new Uri("https://robertsspaceindustries.com") };
            var lResponse = await _httpClient.GetAsync(aAuthorUrl);
            var lStringResponse = await lResponse.Content.ReadAsStringAsync();

            var lParser = new HtmlParser();
            var lHTMLdocument = lParser.ParseDocument(lStringResponse);

            var lElementList = lHTMLdocument.QuerySelector("div.thumb");
            var lRes = (lElementList.Children.First() as AngleSharp.Html.Dom.IHtmlImageElement).Source.Replace("about://", string.Empty);
            return lRes;
        }
    }
}
