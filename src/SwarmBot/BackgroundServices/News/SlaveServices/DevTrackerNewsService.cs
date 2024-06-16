using AngleSharp.Common;
using DSharpPlus.Entities;
using SwarmBot.Application;
using SwarmBot.BackgroundServices.News.Interfaces;
using SwarmBot.BackgroundServices.News.Messages;
using SwarmBot.Configuration;
using TGF.Common.Extensions;
using TGF.Common.Net.Http;

namespace SwarmBot.BackgroundServices.News.SlaveServices
{
    internal readonly struct DevPostType
    {
        public const string PatchNotes = "Patch Notes";
        public const string Announcements = "Announcements";
    }

    /// <summary>
    /// Service that will get the last news from the StarCitizen devtracker resource by reading the HTML and notifying the differences on Discord periodically.
    /// (Has to be like since there is not any RSS available for this resource)
    /// </summary>
    internal class DevTrackerNewsService : DiscordBotNewsServiceBase<DevTrackerNewsMessage>, INewsWebTracker<DevTrackerNewsMessage>
    {
        private readonly BotNewsConfig _botNewsConfig;
        public DevTrackerNewsService(IHttpClientFactory aHttpClientFactory, BotNewsConfig aBotNewsConfig)
        {
            mLastGetElapsedTime = DateTimeOffset.Now;
            _botNewsConfig = aBotNewsConfig;
            mNewsTopicConfig = aBotNewsConfig.DevTracker;
            mTimedHttpClientProvider = new TimedHttpClientProvider(aHttpClientFactory, new TimeSpan(1, 0, 0), aBaseAddress: aBotNewsConfig.BaseResourceAddress);
        }

        #region Overrides

        public override async Task InitAsync(ISwarmBotChannelsService aDiscordChannelsService, TimeSpan aTimeout)
        {
            await base.InitAsync(aDiscordChannelsService, aTimeout);
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
                var lContent = DiscordBotNewsExtensions.GetContentFromHTMLKeyAsArray(sourceDictionary, "TextContent");
                lCurrentContentList.Add(GetMessageFromContentStringList(lContent, sourceDictionary["PathName"][1..]));
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

        #region Private

        /// <summary>
        /// Gets a new instance of <see cref="DevTrackerNewsMessage"/> from an string array from the HTML resource that contains the needed information.
        /// </summary>
        /// <param name="aContentStringList">string array from the HTML resource that contains the needed information.</param>
        /// <param name="aSourceLink">string with the original source link to the news notified in this message that is being build.</param>
        /// <returns>A new instance of <see cref="DevTrackerNewsMessage"/>.</returns>
        private DevTrackerNewsMessage GetMessageFromContentStringList(string[] aContentStringList, string aSourceLink)
            => new DevTrackerNewsMessage()
            {
                /// [1]=autor, [3]=howLongAgo, [4]=group, [5]=title, [6]=desc
                Author = aContentStringList[1],
                Group = aContentStringList[4],
                Title = aContentStringList[5],
                Description = 6 < aContentStringList.Length ? aContentStringList[6] : string.Empty, //description may be empty when devs post only a picture or gif.
                SourceLink = aSourceLink
            };

        /// <summary>
        /// Gets the color of the message based on <see cref="DevTrackerNewsMessage.Group"/> property.
        /// </summary>
        /// <param name="aDevTrackerNewsMessage">Source <see cref="DevTrackerNewsMessage"/>.</param>
        /// <returns>Associated <see cref="DiscordColor"/>.</returns>
        private static DiscordColor GetNewsMessageColor(DevTrackerNewsMessage aDevTrackerNewsMessage)
        {
            return aDevTrackerNewsMessage.Group switch
            {
                DevPostType.PatchNotes => DiscordColor.Yellow,
                DevPostType.Announcements => DiscordColor.CornflowerBlue,
                _ => DiscordColor.Gray,
            };
        }

        #endregion

    }
}
