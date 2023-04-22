using AngleSharp.Common;
using DSharpPlus.Entities;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using Microsoft.Extensions.Configuration;
using TGF.Common.Extensions;

namespace MandrilBot.News
{
    /// <summary>
    /// Provides all necesary logic of a service that will get the last news from the StarCitizen devtracker resource by reading the HTML and notifying the differences on Discord periodically.
    /// (Has to be like since there is not any RSS available for this resource)
    /// </summary>
    public class DiscordBotNewsService : IDiscordBotNewsService
    {
        private readonly BotNewsConfig _botNewsConfig;
        private readonly HttpClient _httpClient;

        private DiscordChannel _devTrackerNewsChannel;
        private List<DevTrackerNewsMessage> _lastMessageList;

        public DiscordBotNewsService(IConfiguration aConfiguration, HttpClient aHttpClient)
        {
            var lBotNewsConfig = new BotNewsConfig();
            aConfiguration.Bind("BotNews", lBotNewsConfig);

            _botNewsConfig = lBotNewsConfig;
            _botNewsConfig.CitizensPath += "/";

            _httpClient = aHttpClient;
            _httpClient.BaseAddress = new Uri(lBotNewsConfig.BaseResourceAddress);
        }

        /// <summary>
        /// Initializes the instance of this service with the requiered missin information it requieres to start working.
        /// </summary>
        /// <param name="aHttpClient"><see cref="HttpClient"/> that will be used to get the news in every Tick.</param>
        /// <param name="aMandrilDiscordBot">Reference to the Discord bot that will send the notification messages.</param>
        /// <param name="aChannelName">Name of the <see cref="DiscordChannel"/> to send the notification messages when available.</param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        public async Task InitAsync(IChannelsController aDiscordChannelsControllerService)
        {
            var lNewsChannelResult = (await (aDiscordChannelsControllerService as ChannelsController).GetDiscordChannel(channel => channel.Name == _botNewsConfig.DevTracker.DiscordChannel));
            if (!lNewsChannelResult.IsSuccess)
                throw new Exception($"Error fetching the SC news channel: {lNewsChannelResult}");

            _devTrackerNewsChannel = lNewsChannelResult.Value;
            _lastMessageList = await GetLastMessageListAsync();
        }

        /// <summary>
        /// Executes one Tick checking if there are news and sending the updates to the designated discord channel if any.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        public async Task TickExecute(CancellationToken aCancellationToken)
        {
            var lUpdateMessageList = await GetUpdatesAsync();
            if (lUpdateMessageList.IsNullOrEmpty()) return;

            await lUpdateMessageList.ParallelForEachAsync(
                MandrilDiscordBot._maxDegreeOfParallelism,
                update => SendMessage(update),
                aCancellationToken
                );
        }

        /// <summary>
        /// Get a List of the last Messages published in the resource of this tracker.
        /// </summary>
        /// <returns>List of the last Messages published in the resource of this tracker.</returns>
        private async Task<List<DevTrackerNewsMessage>> GetLastMessageListAsync()
        {
            var lHTMLdocument = await DiscordBotNewsExtensions.GetHTMLAsync(_httpClient, _botNewsConfig.DevTracker.ResourcePath);
            var lElementList = lHTMLdocument.QuerySelector("div.devtracker-list.js-devtracker-list")?.QuerySelector(".devtracker-list");
            var lDictionaryData = lElementList.Children.Select(y => y.ToDictionary()).ToList();

            List<DevTrackerNewsMessage> lCurrentContentList = new List<DevTrackerNewsMessage>();

            lDictionaryData.ForEach(sourceDictionary =>
            {
                /// [1]=autor, [3]=date, [4]=group, [5]=title, [6]=desc
                var lContent = sourceDictionary["TextContent"]
                                .Split('\n')
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .Select(x => x.Trim())
                                .ToArray();
                lCurrentContentList.Add(new DevTrackerNewsMessage()
                {
                    Author = lContent[1],
                    Date = lContent[3],
                    Group = lContent[4],
                    Title = lContent[5],
                    Description = lContent[6],
                    SourceLink = sourceDictionary["PathName"][1..]
                });
            });

            return lCurrentContentList;
        }

        /// <summary>
        /// Gets the last news that were not notified yet in Discord.
        /// </summary>
        /// <returns>List with the last news that were not notified yet in Discord.</returns>
        private async Task<List<DevTrackerNewsMessage>> GetUpdatesAsync()
        {
            var lContentList = await GetLastMessageListAsync();
            var lRes = lContentList.Except(_lastMessageList, new DevTrackerNewsMessageComparer())?//Need to ignore Date in the except GetHash using DevTrackerNewsMessageComparer
                .ToList();

            if (lRes.Count > 0)
                _lastMessageList = lContentList;

            return lRes;
        }

        /// <summary>
        /// Sends message in the designated Diescord news channel about the given <see cref="DevTrackerNewsMessage"/>.
        /// </summary>
        /// <param name="aDevTrackerNewsMessage">Source <see cref="DevTrackerNewsMessage"/>.</param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        private async Task SendMessage(DevTrackerNewsMessage aDevTrackerNewsMessage)
        {

            await _devTrackerNewsChannel.SendMessageAsync(new DiscordMessageBuilder()
            {
                Embed = new DiscordEmbedBuilder()
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        Name = aDevTrackerNewsMessage.Author,
                        Url = _httpClient.BaseAddress + _botNewsConfig.CitizensPath + aDevTrackerNewsMessage.Author,
                        IconUrl = _httpClient.BaseAddress + await DiscordBotNewsExtensions.GetCitizenImageLink(_httpClient, _botNewsConfig.CitizensPath + aDevTrackerNewsMessage.Author),
                    },
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Height = 10, Width = 10, Url = _botNewsConfig.SpectrumLogoUri },
                    Title = aDevTrackerNewsMessage.Title,
                    Description = aDevTrackerNewsMessage.Description,
                    Url = _httpClient.BaseAddress + aDevTrackerNewsMessage.SourceLink,
                    Color = GetNewsMessageColor(aDevTrackerNewsMessage)
                }
            });

        }

        /// <summary>
        /// Gets the color of the message based on <see cref="DevTrackerNewsMessage.Group"/> property.
        /// </summary>
        /// <param name="aDevTrackerNewsMessage">Source <see cref="DevTrackerNewsMessage"/>.</param>
        /// <returns>Associated <see cref="DiscordColor"/>.</returns>
        private DiscordColor GetNewsMessageColor(DevTrackerNewsMessage aDevTrackerNewsMessage)
        {
            switch (aDevTrackerNewsMessage.Group)
            {
                case "Patch Notes": return DiscordColor.Yellow;
                case "Announcements": return DiscordColor.MidnightBlue;
                default: return DiscordColor.CornflowerBlue;
            }
        }

    }

}
