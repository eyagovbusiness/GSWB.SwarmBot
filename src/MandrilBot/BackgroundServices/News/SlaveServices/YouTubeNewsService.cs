using AngleSharp.Common;
using DSharpPlus.Entities;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using MandrilBot.BackgroundServices.News.Interfaces;
using MandrilBot.BackgroundServices.News.Messages;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using TGF.CA.Infrastructure.Security.Secrets.Vault;
using TGF.Common.Extensions;



namespace MandrilBot.BackgroundServices.News.SlaveServices
{
    /// <summary>
    /// Service that will get from the youtube API v3 the last new videos from the StarCitizen official youtube channel.
    /// </summary>
    internal class YouTubeNewsService : DiscordBotNewsServiceBase<YouTubeNewsMessage>, INewsWebTracker<YouTubeNewsMessage>
    {
        private YouTubeService _youTubeService;
        private readonly ISecretsManager _secretsManager;

        public YouTubeNewsService(ISecretsManager aSecretsManager, BotNewsConfig aBotNewsConfig)
        {
            _secretsManager = aSecretsManager;
            mLastGetElapsedTime = DateTime.UtcNow;
            mNewsTopicConfig = aBotNewsConfig.YouTubeTracker;
            //To get a channel id GET => https://www.googleapis.com/youtube/v3/channels?key={YOUR_API_KEY}&forUsername={TARGET_CHANNEL_NAME}&part=id
        }

        #region Overrides

        public override async Task InitAsync(IChannelsController aDiscordChannelsControllerService, TimeSpan aTimeout)
        {
            await base.InitAsync(aDiscordChannelsControllerService, aTimeout);
            _youTubeService = await GetNewYouTubeService();
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

        #endregion

        #region INewsService

        /// <summary>
        /// Get the latest three uploaded videos from the channel.
        /// </summary>
        /// <returns></returns>
        public async Task<List<YouTubeNewsMessage>> GetLastMessageListAsync()
        {
            var ActivitiesListRequest = _youTubeService.Activities.List("contentDetails,snippet");
            ActivitiesListRequest.ChannelId = (mNewsTopicConfig as YouTubeNewsConfig).TargetChannelId;
            ActivitiesListRequest.MaxResults = 8; // Retrieve the last 5 videos(some results are not "uploaded", so we need to pull 8 items to get the last 5 uploaded videos

            var ActivitiesListResponse = await ActivitiesListRequest.ExecuteAsync();

            List<YouTubeNewsMessage> lCurrentContentList = new();
            if (ActivitiesListResponse == null || ActivitiesListResponse.Items.IsNullOrEmpty())//If could not get the news resource return empty discord message list
                return new List<YouTubeNewsMessage>(); //mLastGetElapsedTime will not be updated and healtcheck will update health if proceeds
            mLastGetElapsedTime = DateTimeOffset.Now;

            return ActivitiesListResponse.Items
                   .Where(item => item.Snippet.Type == "upload")//process only video upload activities
                   .Select(item => GetMessageFromSearchResult(item))
                   .ToList();
        }

        public async Task<List<YouTubeNewsMessage>> GetUpdatesAsync()
        {
            var lContentList = await GetLastMessageListAsync();
            var lRes = lContentList
                .Except(mLastMessageList, new YouTubeNewsMessageComparer())?
                .ToList();

            if (lRes.Count > 0)
                mLastMessageList = lContentList;

            return lRes;
        }

        public async Task SendMessage(YouTubeNewsMessage aYouTubeNewsMessage)
            => await mNewsChannel.SendMessageAsync(new DiscordMessageBuilder()
            {
                Embed = new DiscordEmbedBuilder()
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor()
                    {
                        Name = "YouTube-StarCitizen",
                        Url = "https://www.youtube.com/@RobertsSpaceInd/videos",
                        IconUrl = "https://cdn-icons-png.flaticon.com/512/3670/3670163.png"
                    },
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = aYouTubeNewsMessage.ThumbnailLink },
                    Title = aYouTubeNewsMessage.Title,
                    Description = aYouTubeNewsMessage.Description,
                    Url = aYouTubeNewsMessage.VideoLink,
                    Color = DiscordColor.Red
                }
            });

        #endregion

        #region Private

        /// <summary>
        /// Gets a new instance of <see cref="YouTubeNewsMessage"/> the youtube api response.
        /// </summary>
        /// <param name="aSearchResult">youtube api result.</param>
        /// <returns>A new instance of <see cref="YouTubeNewsMessage"/>.</returns>
        private YouTubeNewsMessage GetMessageFromSearchResult(Activity aActivityResponse)
        => new()
        {
            Title = aActivityResponse.Snippet.Title,
            Description = aActivityResponse.Snippet.Description,
            VideoLink = mNewsTopicConfig.ResourcePath + aActivityResponse.ContentDetails.Upload.VideoId,
            ThumbnailLink = aActivityResponse.Snippet.Thumbnails.High.Url
        };

        /// <summary>
        /// Set up and get the YouTube Data API service.
        /// </summary>
        /// <returns></returns>
        private async Task<YouTubeService> GetNewYouTubeService()
        => new(new BaseClientService.Initializer()
        {
            ApiKey = (await _secretsManager.GetValueObject("youtube", "ApiKey")).ToString(),
            ApplicationName = "SCYouTubeTracker"
        });

        #endregion

    }
}
