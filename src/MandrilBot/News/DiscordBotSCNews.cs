using AngleSharp.Common;
using AngleSharp.Html.Parser;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using MandrilBot.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TGF.Common.Extensions;

namespace MandrilBot.News
{
    public class DiscordBotSCNews
    {
        public struct DevTrackerNewsMessage 
        {
            /// <summary>
            /// Developer who wrote the post.
            /// </summary>
            public string Author;
            /// <summary>
            /// The HTML does not brin any DateTime, but how long ago the post was made in format ("30 minutes ago" or "2 hours ago"..).
            /// </summary>
            public string Date;
            /// <summary>
            /// Title of the post.
            /// </summary>
            public string Title;
            /// <summary>
            /// Actual content of the post.
            /// </summary>
            public string Description;
            /// <summary>
            /// Official source link of the post.
            /// </summary>
            public string SourceLink;

        }
        /// <summary>
        /// Custom Equality comparer for DevTrackerNewsMessage needed to ignore Date as it is changing every hour or minute, it depends see <see cref="DevTrackerNewsMessage.Date"/>
        /// </summary>
        public class DevTrackerNewsMessageComparer : IEqualityComparer<DevTrackerNewsMessage>
        {
            public bool Equals(DevTrackerNewsMessage x, DevTrackerNewsMessage y)
            {
                return x.Author == y.Author
                    && x.Title == y.Title
                    && x.Description == y.Description
                    && x.SourceLink == y.SourceLink;
            }

            public int GetHashCode([DisallowNull] DevTrackerNewsMessage lObj)
            {
                return HashCode.Combine(lObj.Author, lObj.Title, lObj.Description, lObj.SourceLink);
            }
        }

        public class DevTrackerNews
        {
            private readonly string _resourcePath;
            private readonly string _citizensPath;

            private DiscordChannel _devTrackerNewsChannel;
            private HttpClient _httpClient;
            private List<DevTrackerNewsMessage> _lastMessageList;

            public DevTrackerNews(BotNewsConfig lBotNewsConfig)
            {
                _resourcePath = lBotNewsConfig.DevTracker.ResourcePath;
                _citizensPath = lBotNewsConfig.CitizensPath + "/";
            }

            private async Task<List<DevTrackerNewsMessage>> GetLastMessageListAsync()
            {
                var lHTMLdocument = await DiscordBotSCNewsExtensions.GetHTMLAsync(_resourcePath);
                var lElementList = lHTMLdocument.QuerySelector("div.devtracker-list.js-devtracker-list")?.QuerySelector(".devtracker-list");
                var lDictionaryData = lElementList.Children.Select(y => y.ToDictionary()).ToList();

                List<DevTrackerNewsMessage> lCurrentContentList = new List<DevTrackerNewsMessage>();

                lDictionaryData.ForEach(sourceDictionary =>
                {
                    /// [1]=autor, [3]=date, [4]=category, [5]=title, [6]=desc
                    var lContent = sourceDictionary["TextContent"]
                                    .Split('\n')
                                    .Where(x => !string.IsNullOrWhiteSpace(x))
                                    .Select(x => x.Trim())
                                    .ToArray();
                    lCurrentContentList.Add(new DevTrackerNewsMessage()
                    {
                        Author = lContent[1],
                        Date = lContent[3],
                        Title = lContent[5],
                        Description = lContent[6],
                        SourceLink = sourceDictionary["PathName"][1..]
                    });
                });

                return lCurrentContentList;
            }

            public async Task<List<DevTrackerNewsMessage>> GetUpdatesAsync()
            {
                var lContentList = await GetLastMessageListAsync();
                var lRes = lContentList.Except(_lastMessageList, new DevTrackerNewsMessageComparer())?//Need to ignore Date in the except GetHash using DevTrackerNewsMessageComparer
                    .ToList();

                if(lRes.Count > 0 )
                    _lastMessageList = lContentList;

                return lRes;
            }

            public async Task InitAsync(HttpClient aHttpClient, IMandrilDiscordBot aMandrilDiscordBot, string aChannelName)
            {
                _devTrackerNewsChannel = (await (aMandrilDiscordBot as MandrilDiscordBot).TryGetDiscordChannelAsync(aChannelName)).Value;
                _httpClient = aHttpClient;
                _lastMessageList = await GetLastMessageListAsync();
            }

            public async Task TickExecute(CancellationToken aCancellationToken)
            {
                var lUpdateMessageList = await GetUpdatesAsync();
                if (lUpdateMessageList.IsNullOrEmpty()) return;

                await lUpdateMessageList.ParallelForEachAsync(
                    MandrilDiscordBot._maxDegreeOfParallelism,
                    async update => await _devTrackerNewsChannel.SendMessageAsync(new DiscordMessageBuilder()
                    {
                        Embed = new DiscordEmbedBuilder()
                        {
                            Author = new DiscordEmbedBuilder.EmbedAuthor()
                            {
                                Name = update.Author,
                                Url = _httpClient.BaseAddress + _citizensPath + update.Author,
                                IconUrl = _httpClient.BaseAddress + await DiscordBotSCNewsExtensions.GetCitizenImageLink(_citizensPath + update.Author),
                            },
                            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Height = 10, Width = 10, Url = "https://starcitizen.tools/images/thumb/0/06/Spectrum.jpg/300px-Spectrum.jpg" },
                            Title = update.Title,
                            Description = update.Description,
                            Url = _httpClient.BaseAddress + update.SourceLink
                        }
                    }),
                    aCancellationToken
                    );
            }

        }

    }
}
