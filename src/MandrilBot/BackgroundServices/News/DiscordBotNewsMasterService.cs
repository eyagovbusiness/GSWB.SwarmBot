using Mandril.Application;
using MandrilBot.BackgroundServices.News.SlaveServices;
using MandrilBot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TGF.CA.Application;
using TGF.Common.Extensions;

namespace MandrilBot.BackgroundServices.News
{
    /// <summary>
    /// Master DiscordBotNewsService that orchestrates all the <see cref="IDiscordBotNewsService"/> instances each one providing news on discord from tracking specific web resources and notifying on discord via messages in the designated channel for each news service.
    /// </summary>
    /// <remarks>Depends on <see cref="IMandrilDiscordBot"/> and <see cref="IMandrilChannelsService"/>.</remarks>
    public partial class DiscordBotNewsMasterService : IDiscordBotNewsService
    {
        private readonly BotNewsConfig _botNewsConfig; //Main configuration that will be used by all the slave news services.
        private readonly IDiscordBotNewsService[] _newsResourceTrackerList; //Array with all the slave news services.

        /// <summary>
        /// Master DiscordBotNewsService that orchestrates all the <see cref="IDiscordBotNewsService"/> instances each one providing news on discord from tracking specific web resources and notifying on discord via messages in the designated channel for each news service.
        /// </summary>
        /// <param name="aConfiguration"><see cref="IConfiguration"/> from the ASP.NET DI container.</param>
        /// <param name="aHttpClientFactory"><see cref="IHttpClientFactory"/> from the ASP.NET DI container.(needs WebHostBuilder.Services.AddHttpClient() in the web application builder.</param>
        public DiscordBotNewsMasterService(IConfiguration aConfiguration, IHttpClientFactory aHttpClientFactory, ISecretsManager aSecretsManager)
        {
            var lBotNewsConfig = new BotNewsConfig();
            aConfiguration.Bind("BotNews", lBotNewsConfig);

            _botNewsConfig = lBotNewsConfig;
            _botNewsConfig.CitizensPath += "/";

            _newsResourceTrackerList = new IDiscordBotNewsService[]
            {
                new DevTrackerNewsService(aHttpClientFactory, _botNewsConfig),
                new CommLinkNewsService(aHttpClientFactory, _botNewsConfig),
                new RSIStatusNewsService(aHttpClientFactory, _botNewsConfig),
                new YouTubeNewsService(aSecretsManager, _botNewsConfig)
            };
        }

        #region IDiscordBotNewsService

        /// <summary>
        /// Triggers in parallel every asynchronous <see cref="IDiscordBotNewsService.InitAsync(IMandrilChannelsService, TimeSpan)"/> of all the slave news services.
        /// </summary>
        /// <param name="aDiscordChannelsService"></param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        public async Task InitAsync(IMandrilChannelsService aDiscordChannelsService, TimeSpan aTimeout)
            => await _newsResourceTrackerList.ParallelForEachAsync(
                MandrilDiscordBot._maxDegreeOfParallelism,
                tracker => tracker.InitAsync(aDiscordChannelsService, aTimeout)
                );

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

        public HealthCheckResult GetHealthCheck(CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();

            var lHealthIssuesList = _newsResourceTrackerList
                .Select(newsTracker => newsTracker.GetHealthCheck())
                .ToArray();

            string lJoinedHealthDescriptions = string.Join($" {Environment.NewLine}", lHealthIssuesList.Select(healthCheckResult => healthCheckResult.Description));
            return lHealthIssuesList.All(healthCheckResult => healthCheckResult.Status == Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy)
                ? HealthCheckResult.Healthy(lJoinedHealthDescriptions)
                : HealthCheckResult.Degraded(lJoinedHealthDescriptions);

        }

        #endregion

    }

}
