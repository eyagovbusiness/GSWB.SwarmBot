using DSharpPlus.Entities;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using MandrilBot.News.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGF.Common.Net.Http;

namespace MandrilBot.News.SlaveServices
{
    /// <summary>
    /// Abstract base class for all the slave DiscordBotNewsServices with some default common behaviours for slaves from <see cref="IDiscordBotNewsService"/> and needed common variables.
    /// </summary>
    /// <typeparam name="TMessageStruct"></typeparam>
    public abstract class DiscordBotNewsServiceBase<TMessageStruct> : IDiscordBotNewsService // T of type DevTrackerNewsMessage or other messages
    {
        protected TimedHttpClientProvider mTimedHttpClientProvider;
        protected DiscordChannel mNewsChannel;
        protected List<TMessageStruct> mLastMessageList;
        protected NewsTopicConfig mNewsTopicConfig;
        protected DateTimeOffset mLastGetElapsedTime = DateTimeOffset.Now;
        protected int mMaxGetElapsedTime = 60;

        #region IDiscordBotNewsService

        public virtual async Task InitAsync(IChannelsController aDiscordChannelsControllerService)
        {
            var lNewsChannelResult = await (aDiscordChannelsControllerService as ChannelsController).GetDiscordChannel(channel => channel.Id == mNewsTopicConfig.DiscordChannelId);
            if (!lNewsChannelResult.IsSuccess)
                throw new Exception($"Error fetching the SC news channel: {lNewsChannelResult}");
            mNewsChannel = lNewsChannelResult.Value;

        }

        public virtual Task TickExecute(CancellationToken aCancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual void SetHealthCheck_Healthy_MaxGetElapsedTime_InSeconds(int aSeconds)
            => mMaxGetElapsedTime = aSeconds;

        public virtual HealthCheckResult GetHealthCheck(CancellationToken aCancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
