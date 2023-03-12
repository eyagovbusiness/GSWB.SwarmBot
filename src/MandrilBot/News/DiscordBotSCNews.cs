using AngleSharp.Common;
using AngleSharp.Html.Parser;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TGF.Common.Extensions;

namespace MandrilBot.News
{
    public class DiscordBotSCNews
    {
        public struct DevTrackerNewsMessage
        {
            public string Author;
            public string Date;
            public string Title;
            public string Description;
            public string SourceLink;
        }

        public class DevTrackerNews
        {
            private readonly string _resourcePath;
            private readonly string _citizensPath;
            private readonly DiscordChannel _devTrackerNewsChannel;
            private readonly HttpClient _httpClient;
            private static List<DevTrackerNewsMessage> _lastMessageList;

            public DevTrackerNews(DiscordChannel aDevTrackerNewsChannel, HttpClient ahttpClient, string aResourcePath, string aCitizensPath)
            {
                _resourcePath = aResourcePath;
                _citizensPath = aCitizensPath + "/";
                _httpClient = ahttpClient;
                _devTrackerNewsChannel = aDevTrackerNewsChannel;
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
                        SourceLink = sourceDictionary["PathName"]
                    });
                });

                return lCurrentContentList;
            }

            public async Task<List<DevTrackerNewsMessage>> GetUpdatesAsync()
            {
                var lContentList = await GetLastMessageListAsync();
                var lRes = lContentList.Except(_lastMessageList).ToList();
                _lastMessageList = lContentList;
                return lRes;
            }

            public async Task InitAsync()
            {
                _ = await GetLastMessageListAsync();
            }

            public async Task TickExecute(CancellationToken aCancellationToken)
            {
                try
                {
                    var lUpdateMessageList = await GetUpdatesAsync();

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
                                Url = update.SourceLink
                            }
                        }),
                        aCancellationToken
                        );
                }
                catch (BadRequestException)
                {
                    await _devTrackerNewsChannel.SendMessageAsync("An error occurred, please notify the administrator.");
                }
            }

        }

    }
}
