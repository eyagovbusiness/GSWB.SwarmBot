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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TGF.Common.Extensions;
using TGF.Common.Net.Http;

namespace MandrilBot.News.SlaveServices
{
    /// <summary>
    /// Service that will get the last news from the StarCitizen comm-link resource by reading the HTML and notifying the differences on Discord periodically.
    /// (Has to be like since there is not any RSS available for this resource)
    /// </summary>
    internal class CommLinkNewsService : DiscordBotNewsServiceBase<CommLinkNewsMessage>, INewsWebTracker<CommLinkNewsMessage>
    {
        private readonly BotNewsConfig _botNewsConfig;
        public CommLinkNewsService(IHttpClientFactory aHttpClientFactory, BotNewsConfig aBotNewsConfig)
        {
            _botNewsConfig = aBotNewsConfig;
            mNewsTopicConfig = aBotNewsConfig.CommLink;
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
                ? HealthCheckResult.Degraded(string.Format("The CommLinkNewsService's health is degraded. It was not possible to get the news resource, the last successful get was at {0}", mLastGetElapsedTime))
                : HealthCheckResult.Healthy(string.Format("The CommLinkNewsService is healthy. Last news get was {0} seconds ago.", lElapsedSecondsSinceTheLastGet));
        }

        #endregion

        #region INewsService

        public async Task<List<CommLinkNewsMessage>> GetLastMessageListAsync()
        {
            var lHTMLdocument = await DiscordBotNewsExtensions.GetHTMLAsync(mTimedHttpClientProvider.GetHttpClient(), mNewsTopicConfig.ResourcePath);
            var disct = lHTMLdocument.ToDictionary();
            var lElementList = lHTMLdocument?.QuerySelector("div.hub-blocks");//YOOOO BREAK THIS AND CHECK WHY HEALTH IS STILL OK!!!!

            List<CommLinkNewsMessage> lCurrentContentList = new();
            if (lElementList == null)//If could not get the news resource return empty discord message list
                return lCurrentContentList; //mLastGetElapsedTime will not be updated and healtcheck will update health if proceeds
            mLastGetElapsedTime = DateTimeOffset.Now;

            var lDictionaryData = lElementList.Children.Select(y => y.ToDictionary()).Take(10).ToList();//Take only the newest 10 
            lDictionaryData.ForEach(sourceDictionary =>
            {
                /// [1]=ImageLinkDiv,
                var lInnerContent = sourceDictionary["InnerHtml"]
                                .Split('\n')
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .Select(x => x.Trim())
                                .ToArray(); 
                /// [0]=mediaType, [1]=title, [2]=numComments, [3]=howLongAgo, [4]=desc
                var lContent = sourceDictionary["TextContent"]
                                .Split('\n')
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .Select(x => x.Trim())
                                .ToArray();

                lCurrentContentList.Add(new CommLinkNewsMessage()
                {
                    Title = lContent[1],
                    Description = 4 < lContent.Length ? lContent[4] : string.Empty, //description may be empty
                    SourceLink = sourceDictionary["PathName"][1..],
                    ImageLink = ExtractImageLinkUrl(lInnerContent[1])

                });
            });

            return lCurrentContentList;
        }

        public async Task<List<CommLinkNewsMessage>> GetUpdatesAsync()
        {
            var lContentList = await GetLastMessageListAsync();
            var lRes = lContentList.Except(mLastMessageList, new CommLinkNewsMessageComparer())?//Need to ignore Date in the except GetHash using CommLinkNewsMessageComparer
                .ToList();

            if (lRes.Count > 0)
                mLastMessageList = lContentList;

            return lRes;
        }

        public async Task SendMessage(CommLinkNewsMessage aCommLinkNewsMessage)
        {
            var lBaseAddress = mTimedHttpClientProvider.GetHttpClient().BaseAddress;
            await mNewsChannel.SendMessageAsync(new DiscordMessageBuilder()
            {
                Embed = new DiscordEmbedBuilder()
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        Name = "RSI Comm-Link",
                        Url = lBaseAddress + _botNewsConfig.CommLink.ResourcePath,
                        IconUrl = "https://spng.pngfind.com/pngs/s/90-903191_star-citizen-logo-png-download-transparent-png.png",
                    },
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = aCommLinkNewsMessage.ImageLink },
                    Title = aCommLinkNewsMessage.Title,
                    Description = aCommLinkNewsMessage.Description,
                    Url = lBaseAddress + aCommLinkNewsMessage.SourceLink,
                    Color = DiscordColor.White
                }
            });
        }

        #endregion

        /// <summary>
        /// Extracts the ImageLink url from an string representing the HTML element that contains the link.
        /// </summary>
        /// <param name="aHtmlString"><see cref="string"/> representing the HTML element that contains the link</param>
        /// <returns><see cref="string"/> that represents the image url link </returns>
        private static string ExtractImageLinkUrl(string aHtmlString)
        {
            string lPattern = "url\\('([^']*)'\\)";
            Match lMatch = Regex.Match(aHtmlString, lPattern);

            return lMatch.Success
                ? lMatch.Groups[1].Value
                : default;
        }

    }
}
