using AngleSharp.Common;
using DSharpPlus.Entities;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using MandrilBot.News.Interfaces;
using MandrilBot.News.Messages;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TGF.Common.Extensions;
using TGF.Common.Net.Http;

namespace MandrilBot.News.SlaveServices
{
    /// <summary>
    /// Service that will get the last news from the StarCitizen devtracker resource by reading the HTML and notifying the differences on Discord periodically.
    /// (Has to be like since there is not any RSS available for this resource)
    /// </summary>
    public class DevTrackerNewsService : DiscordBotNewsServiceBase<DevTrackerNewsMessage>, INewsWebTracker<DevTrackerNewsMessage>
    {
        private readonly BotNewsConfig _botNewsConfig;
        public DevTrackerNewsService(IHttpClientFactory aHttpClientFactory, BotNewsConfig aBotNewsConfig)
        {
            _botNewsConfig = aBotNewsConfig;
            mNewsTopicConfig = aBotNewsConfig.DevTracker;
            mTimedHttpClientProvider = new TimedHttpClientProvider(aHttpClientFactory, new TimeSpan(1, 0, 0), aBaseAddress: aBotNewsConfig.BaseResourceAddress);
        }

        #region Overrides

        public override async Task InitAsync(IChannelsController aDiscordChannelsControllerService)
        {
            await base.InitAsync(aDiscordChannelsControllerService);
            mLastMessageList = await GetLastMessageListAsync();
        }

        public override async Task TickExecute(CancellationToken aCancellationToken)
        {
            var lUpdateMessageList = await GetUpdatesAsync();
            if (lUpdateMessageList.IsNullOrEmpty()) return;

            await lUpdateMessageList.ParallelForEachAsync(
                MandrilDiscordBot._maxDegreeOfParallelism,
                update => SendMessage(update),
                aCancellationToken
                );
        }

        public override HealthCheckResult GetHealthCheck(CancellationToken aCancellationToken = default)
        {
            var lElapsedSecondsSinceTheLastGet = (DateTimeOffset.Now - mLastGetElapsedTime).Seconds;
            return lElapsedSecondsSinceTheLastGet > mMaxGetElapsedTime
                ? HealthCheckResult.Degraded(string.Format("The DevTrackerNewsService's health is degraded. It was not possible to get the news resource, the last successful get was at {0}", mLastGetElapsedTime))
                : HealthCheckResult.Healthy(string.Format("The DevTrackerNewsService is healthy. Last news get was {0} seconds ago.", lElapsedSecondsSinceTheLastGet));
        }

        #endregion

        #region INewsService

        public async Task<List<DevTrackerNewsMessage>> GetLastMessageListAsync()
        {
            var lHTMLdocument = await DiscordBotNewsExtensions.GetHTMLAsync(mTimedHttpClientProvider.GetHttpClient(), mNewsTopicConfig.ResourcePath);
            var lElementList = lHTMLdocument?.QuerySelector("div.devtracker-list.js-devtracker-list")?.QuerySelector(".devtracker-list");

            List<DevTrackerNewsMessage> lCurrentContentList = new();
            if (lElementList == null)//If could not get the news resource return empty discord message list
                return lCurrentContentList; //mLastGetElapsedTime will not be updated and healtcheck will update health if proceeds
            mLastGetElapsedTime = DateTimeOffset.Now;

            var lDictionaryData = lElementList.Children.Select(y => y.ToDictionary()).ToList();
            lDictionaryData.ForEach(sourceDictionary =>
            {
                /// [1]=autor, [3]=howLongAgo, [4]=group, [5]=title, [6]=desc
                var lContent = sourceDictionary["TextContent"]
                                .Split('\n')
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .Select(x => x.Trim())
                                .ToArray();
                lCurrentContentList.Add(new DevTrackerNewsMessage()
                {
                    Author = lContent[1],
                    Group = lContent[4],
                    Title = lContent[5],
                    Description = 6 < lContent.Length ? lContent[6] : string.Empty, //description may be empty when devs post only a picture or gif.
                    SourceLink = sourceDictionary["PathName"][1..]
                });
            });

            return lCurrentContentList;
        }

        public async Task<List<DevTrackerNewsMessage>> GetUpdatesAsync()
        {
            var lContentList = await GetLastMessageListAsync();
            var lRes = lContentList.Except(mLastMessageList, new DevTrackerNewsMessageComparer())?//Need to ignore Date in the except GetHash using DevTrackerNewsMessageComparer
                .ToList();

            if (lRes.Count > 0)
                mLastMessageList = lContentList;

            return lRes;
        }

        public async Task SendMessage(DevTrackerNewsMessage aDevTrackerNewsMessage)
        {
            var lBaseAddress = mTimedHttpClientProvider.GetHttpClient().BaseAddress;
            await mNewsChannel.SendMessageAsync(new DiscordMessageBuilder()
            {
                Embed = new DiscordEmbedBuilder()
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        Name = aDevTrackerNewsMessage.Author,
                        Url = lBaseAddress + _botNewsConfig.CitizensPath + aDevTrackerNewsMessage.Author,
                        IconUrl = lBaseAddress + await DiscordBotNewsExtensions.GetCitizenImageLink(mTimedHttpClientProvider.GetHttpClient(), _botNewsConfig.CitizensPath + aDevTrackerNewsMessage.Author),
                    },
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = _botNewsConfig.SpectrumLogoUri },
                    Title = aDevTrackerNewsMessage.Title,
                    Description = aDevTrackerNewsMessage.Description,
                    Url = lBaseAddress + aDevTrackerNewsMessage.SourceLink,
                    Color = GetNewsMessageColor(aDevTrackerNewsMessage)
                }
            });
        }

        #endregion

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
