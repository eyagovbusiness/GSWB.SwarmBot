using AngleSharp.Common;
using DSharpPlus.Entities;
using SwarmBot.Application;
using SwarmBot.BackgroundServices.News.Interfaces;
using SwarmBot.BackgroundServices.News.Messages;
using SwarmBot.Configuration;
using System.Text.RegularExpressions;
using TGF.Common.Extensions;
using TGF.Common.Net.Http;

namespace SwarmBot.BackgroundServices.News.SlaveServices
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
            mLastGetElapsedTime = DateTimeOffset.Now;
            _botNewsConfig = aBotNewsConfig;
            mNewsTopicConfig = aBotNewsConfig.CommLink;
            mTimedHttpClientProvider = new TimedHttpClientProvider(aHttpClientFactory, new TimeSpan(1, 0, 0), aBaseAddress: aBotNewsConfig.BaseResourceAddress);
        }

        #region Overrides

        public override async Task InitAsync(ISwarmBotChannelsService aDiscordChannelsControllerService, TimeSpan aTimeout)
        {
            await base.InitAsync(aDiscordChannelsControllerService, aTimeout);
            mLastMessageList = await GetLastMessageListAsync();
        }

        public override async Task TickExecute(CancellationToken aCancellationToken)
        {
            var lUpdateMessageList = await GetUpdatesAsync();
            if (lUpdateMessageList.IsNullOrEmpty()) return;

            await lUpdateMessageList.ParallelForEachAsync(
                SwarmBotDiscordBot._maxDegreeOfParallelism,
                update => SendMessage(update),
                aCancellationToken
                );
        }

        #endregion

        #region INewsService

        public async Task<List<CommLinkNewsMessage>> GetLastMessageListAsync()
        {
            var lHTMLdocument = await DiscordBotNewsExtensions.GetHTMLAsync(mTimedHttpClientProvider.GetHttpClient(), mNewsTopicConfig.ResourcePath);
            var disct = lHTMLdocument.ToDictionary();
            var lElementList = lHTMLdocument?.QuerySelector("div.hub-blocks");

            List<CommLinkNewsMessage> lCurrentContentList = new();
            if (lElementList == null)//If could not get the news resource return empty discord message list
                return lCurrentContentList; //mLastGetElapsedTime will not be updated and healtcheck will update health if proceeds
            mLastGetElapsedTime = DateTimeOffset.Now;

            var lDictionaryData = lElementList.Children.Select(y => y.ToDictionary()).Take(10).ToList();//Take only the newest 10 
            lDictionaryData.ForEach(sourceDictionary =>
            {
                var lInnerContent = DiscordBotNewsExtensions.GetContentFromHTMLKeyAsArray(sourceDictionary, "InnerHtml");
                var lContent = DiscordBotNewsExtensions.GetContentFromHTMLKeyAsArray(sourceDictionary, "TextContent");
                lCurrentContentList.Add(GetMessageFromContentStringList(lContent, sourceDictionary["PathName"][1..], lInnerContent));
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
                        IconUrl = "https://media3.giphy.com/media/26tk1Qmvy7soIgp7G/200w.gif?cid=6c09b952lk22yoa6ffffpxhgdrtr02chcsv3koaock15hm7p&ep=v1_gifs_search&rid=200w.gif&ct=g"
                    },
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = aCommLinkNewsMessage.ImageLink },
                    Title = aCommLinkNewsMessage.Title + $" ({aCommLinkNewsMessage.MediaType})",
                    Description = aCommLinkNewsMessage.Description,
                    Url = lBaseAddress + aCommLinkNewsMessage.SourceLink,
                    Color = DiscordColor.White
                }
            });
        }

        #endregion

        #region Private

        /// <summary>
        /// Gets a new instance of <see cref="CommLinkNewsMessage"/> from an string array from the HTML resource that contains the needed information.
        /// </summary>
        /// <param name="aContentStringList">string array from the HTML resource that contains the needed information.</param>
        /// <param name="aSourceLink">string with the original source link to the news notified in this message that is being build.</param>
        /// <param name="aInnerContentStringList">string array from the inner HTML resource that contains the needed information.</param>
        /// <returns>A new instance of <see cref="CommLinkNewsMessage"/>.</returns>
        private CommLinkNewsMessage GetMessageFromContentStringList(string[] aContentStringList, string aSourceLink, string[] aInnerContentStringList)
            => new CommLinkNewsMessage()
            {
                ///aContentStringList => [0]=mediaType, [1]=title, [2]=numComments, [3]=howLongAgo, [4]=desc
                MediaType = aContentStringList[0],
                Title = aContentStringList[1],
                Description = 4 < aContentStringList.Length ? aContentStringList[4] : string.Empty, //description may be empty
                SourceLink = aSourceLink,
                /// aInnerContentStringList => [1]=ImageLinkDiv,
                ImageLink = ExtractImageLinkUrl(aInnerContentStringList[1])

            };

        /// <summary>
        /// Extracts the ImageLink url from an string representing the HTML element that contains the link.
        /// </summary>
        /// <param name="aHtmlString"><see cref="string"/> representing the HTML element that contains the link</param>
        /// <returns><see cref="string"/> that represents the image url link </returns>
        private string ExtractImageLinkUrl(string aHtmlString)
        {
            string lPattern = "url\\('([^']*)'\\)";
            Match lMatch = Regex.Match(aHtmlString, lPattern);

            var lResource = lMatch.Success
                ? lMatch.Groups[1].Value
                : default;
            //looks like sometimes RSI uses "media" subdomain with full path and sometimes the main domain with relative path.
            return lResource.StartsWith("https://")
                ? lResource
                : _botNewsConfig.BaseResourceAddress + lResource;
        }

        #endregion

    }
}
