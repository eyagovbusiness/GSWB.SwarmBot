using AngleSharp.Common;
using Consul;
using DSharpPlus.Entities;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Net.Http;
using TGF.Common.Extensions;

namespace MandrilBot.News
{
    /// <summary>
    /// Provides all necesary logic of a service that will get the last news from the StarCitizen devtracker resource by reading the HTML and notifying the differences on Discord periodically.
    /// (Has to be like since there is not any RSS available for this resource)
    /// </summary>
    public partial class DiscordBotNewsService : IDiscordBotNewsService
    {
        private readonly BotNewsConfig _botNewsConfig;
        private readonly HttpClient _httpClient;

        private DiscordChannel mDevTrackerNewsChannel;
        private List<DevTrackerNewsMessage> mLastMessageList;
        private DateTimeOffset mLastGetElapsedTime = DateTimeOffset.Now;
        private int mMaxGetElapsedTime = 60;

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
        /// Sets the number of seconds that the service healtcheck will use to consider if the service is healthy or not
        /// depeding on the elapsed time between the last successful http get from news resource until the time of checking the service health.
        /// </summary>
        /// <param name="aSeconds"></param>
        public void SetHealthCheck_Healthy_MaxGetElapsedTime_InSeconds(int aSeconds)
            => mMaxGetElapsedTime = aSeconds;

        /// <summary>
        /// Gets a HealthCheck information about this service.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>
        /// <see cref="HealthCheckResult"/> healthy if the last get from the news resource was not empty and it was within the provided <see cref="DiscordBotNewsService.mMaxGetElapsedTime"/>.</returns>
        public HealthCheckResult GetHealthCheck(CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            var lElapsedSecondsSinceTheLastGet = (DateTimeOffset.Now - mLastGetElapsedTime).Seconds;

            return lElapsedSecondsSinceTheLastGet > mMaxGetElapsedTime
                ? HealthCheckResult.Degraded(string.Format("The service's health is degraded. It was not possible to get the news resource, the last successful get was at {0}", mLastGetElapsedTime))
                : HealthCheckResult.Healthy(string.Format("The service is healthy. Last news get was {0} seconds ago.", lElapsedSecondsSinceTheLastGet));
                
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
            var lNewsChannelResult = (await (aDiscordChannelsControllerService as ChannelsController).GetDiscordChannel(channel => channel.Id == _botNewsConfig.DevTracker.DiscordChannelId));
            if (!lNewsChannelResult.IsSuccess)
                throw new Exception($"Error fetching the SC news channel: {lNewsChannelResult}");

            mDevTrackerNewsChannel = lNewsChannelResult.Value;
            mLastMessageList = await GetLastMessageListAsync();
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
            var lElementList = lHTMLdocument?.QuerySelector("div.devtracker-list.js-devtracker-list")?.QuerySelector(".devtracker-list");

            List<DevTrackerNewsMessage> lCurrentContentList = new();
            if (lElementList == null)//If could not get the news resource return empty discord message list
                return lCurrentContentList; //mLastGetElapsedTime will not be updated and healtcheck will update healt if proceeds
            mLastGetElapsedTime = DateTimeOffset.Now;

            var lDictionaryData = lElementList.Children.Select(y => y.ToDictionary()).ToList();
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
                    Description = 6 < lContent.Length ? lContent[6] : string.Empty, //description may be empty when devs post only a picture or gif.
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
            var lRes = lContentList.Except(mLastMessageList, new DevTrackerNewsMessageComparer())?//Need to ignore Date in the except GetHash using DevTrackerNewsMessageComparer
                .ToList();

            if (lRes.Count > 0)
                mLastMessageList = lContentList;

            return lRes;
        }

        /// <summary>
        /// Sends message in the designated Diescord news channel about the given <see cref="DevTrackerNewsMessage"/>.
        /// </summary>
        /// <param name="aDevTrackerNewsMessage">Source <see cref="DevTrackerNewsMessage"/>.</param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        private async Task SendMessage(DevTrackerNewsMessage aDevTrackerNewsMessage)
        {

            await mDevTrackerNewsChannel.SendMessageAsync(new DiscordMessageBuilder()
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
        private static DiscordColor GetNewsMessageColor(DevTrackerNewsMessage aDevTrackerNewsMessage)
        {
            return aDevTrackerNewsMessage.Group switch
            {
                "Patch Notes" => DiscordColor.Yellow,
                "Announcements" => DiscordColor.CornflowerBlue,
                _ => DiscordColor.MidnightBlue,
            };
        }

    }

}
