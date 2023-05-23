using DSharpPlus.Entities;
using MandrilBot.BackgroundServices.News.Interfaces;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TGF.Common.Net.Http;

namespace MandrilBot.BackgroundServices.News.SlaveServices
{
    /// <summary>
    /// Abstract base class for all the slave DiscordBotNewsServices with some default common behaviours for slaves from <see cref="IDiscordBotNewsService"/> and needed common variables.
    /// </summary>
    /// <typeparam name="TMessageStruct"></typeparam>
    internal abstract class DiscordBotNewsServiceBase<TMessageStruct> : IDiscordBotNewsService // T from News.Messages
    {
        protected TimedHttpClientProvider mTimedHttpClientProvider;
        protected DiscordChannel mNewsChannel;
        protected List<TMessageStruct> mLastMessageList;
        protected NewsTopicConfig mNewsTopicConfig;
        protected int mMaxGetElapsedTime = 20;
        protected DateTimeOffset mLastGetElapsedTime;

        #region IDiscordBotNewsService

        public virtual async Task InitAsync(IChannelsController aDiscordChannelsControllerService, TimeSpan aTimeout)
        {
            mTimedHttpClientProvider.SetTimeout(aTimeout);
            var lNewsChannelResult = await (aDiscordChannelsControllerService as ChannelsController).GetDiscordChannel(channel => channel.Id == mNewsTopicConfig.DiscordChannelId);
            if (!lNewsChannelResult.IsSuccess)
                throw new Exception($"Error fetching the SC news channel: {lNewsChannelResult}");
            mNewsChannel = lNewsChannelResult.Value;

        }

        public virtual HealthCheckResult GetHealthCheck(CancellationToken aCancellationToken = default)
        {
            var lElapsedSecondsSinceTheLastGet = (DateTimeOffset.Now - mLastGetElapsedTime).TotalSeconds;
            return lElapsedSecondsSinceTheLastGet > mMaxGetElapsedTime
                ? HealthCheckResult.Degraded($"The {GetType().Name}'s health is degraded. Failed to get the news resource, the last successful get was at {mLastGetElapsedTime}.")
                : HealthCheckResult.Healthy($"The {GetType().Name} is healthy. Last news get was {lElapsedSecondsSinceTheLastGet.ToString("0.0")} seconds ago.");
        }

        public virtual Task TickExecute(CancellationToken aCancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual void SetHealthCheck_Healthy_MaxGetElapsedTime_InSeconds(int aSeconds)
            => mMaxGetElapsedTime = aSeconds;

        #endregion

    }
}
