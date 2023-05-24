using MandrilBot.BackgroundServices.News.Interfaces;
using MandrilBot.BackgroundServices.News.SlaveServices;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TGF.Common.Extensions;

namespace MandrilBot.BackgroundServices.News
{
    /// <summary>
    /// Master DiscordBotNewsService that orchestrates all the <see cref="IDiscordBotNewsService"/> instances each one providing news on discord from tracking specific web resources and notifying on discord via messages in the designated channel for each news service.
    /// </summary>
    public partial class DiscordBotNewsMasterService : IDiscordBotNewsService
    {
        private readonly BotNewsConfig _botNewsConfig; //Main configuration that will be used by all the slave news services.
        private readonly IDiscordBotNewsService[] _newsResourceTrackerList; //Array with all the slave news services.

        /// <summary>
        /// Master DiscordBotNewsService that orchestrates all the <see cref="IDiscordBotNewsService"/> instances each one providing news on discord from tracking specific web resources and notifying on discord via messages in the designated channel for each news service.
        /// </summary>
        /// <param name="aConfiguration"><see cref="IConfiguration"/> from the ASP.NET DI container.</param>
        /// <param name="aHttpClientFactory"><see cref="IHttpClientFactory"/> from the ASP.NET DI container.(needs WebHostBuilder.Services.AddHttpClient() in the web application builder.</param>
        public DiscordBotNewsMasterService(IConfiguration aConfiguration, IHttpClientFactory aHttpClientFactory)
        {
            var lBotNewsConfig = new BotNewsConfig();
            aConfiguration.Bind("BotNews", lBotNewsConfig);

            _botNewsConfig = lBotNewsConfig;
            _botNewsConfig.CitizensPath += "/";

            _newsResourceTrackerList = new IDiscordBotNewsService[]
            {
                new DevTrackerNewsService(aHttpClientFactory, _botNewsConfig),
                new CommLinkNewsService(aHttpClientFactory, _botNewsConfig),
                new RSIStatusNewsService(aHttpClientFactory, _botNewsConfig)
            };
        }

        #region IDiscordBotNewsService

        /// <summary>
        /// Triggers in parallel every asynchronous <see cref="IDiscordBotNewsService.InitAsync(IChannelsController)"/> of all the slave news services.
        /// </summary>
        /// <param name="aDiscordChannelsControllerService"></param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        public async Task InitAsync(IChannelsController aDiscordChannelsControllerService, TimeSpan aTimeout)
            => await _newsResourceTrackerList.ParallelForEachAsync(
                MandrilDiscordBot._maxDegreeOfParallelism,
                tracker => tracker.InitAsync(aDiscordChannelsControllerService, aTimeout)
                );

        /// <summary>
        /// Sets the number of seconds that all the slave services healtchecks will use to consider if the slave service is healthy or not
        /// depeding on the elapsed time between the last successful http get from news resource until the time of checking the service health.
        /// </summary>
        /// <param name="aSeconds"></param>
        public void SetHealthCheck_Healthy_MaxGetElapsedTime_InSeconds(int aSeconds)
            => _newsResourceTrackerList.ForEach(newsService => newsService.SetHealthCheck_Healthy_MaxGetElapsedTime_InSeconds(aSeconds));

        /// <summary>
        /// Triggers in parallel every asynchronous <see cref="IDiscordBotNewsService.TickExecute(CancellationToken)"/> of all the slave news services.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        public async Task TickExecute(CancellationToken aCancellationToken)
            => await _newsResourceTrackerList.ParallelForEachAsync(
                    MandrilDiscordBot._maxDegreeOfParallelism,
                    tracker => tracker.TickExecute(aCancellationToken),
                    aCancellationToken
                    );

        /// <summary>
        /// Gets a HealthCheck information about this service merging all the individual healthChecks from the slave news services.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>
        /// <see cref="HealthCheckResult"/> healthy if all the slave news services are health, otherwise degraded if at least one is unhealthy.</returns>
        public HealthCheckResult GetHealthCheck(CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();

            var lHealthIssuesList = _newsResourceTrackerList
                .Select(newsTrakcer => newsTrakcer.GetHealthCheck())
                .ToArray();

            string lJoinedHealthDescriptions = string.Join($" {Environment.NewLine}", lHealthIssuesList.Select(healthCheckResult => healthCheckResult.Description));
            return lHealthIssuesList.All(healthCheckResult => healthCheckResult.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy)
                ? HealthCheckResult.Healthy(lJoinedHealthDescriptions)
                : HealthCheckResult.Degraded(lJoinedHealthDescriptions);

        }

        #endregion

    }

}
