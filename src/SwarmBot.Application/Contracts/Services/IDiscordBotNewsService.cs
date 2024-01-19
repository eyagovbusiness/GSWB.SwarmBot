using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SwarmBot.Application
{
    /// <summary>
    /// Public interface to define the minimum behaviors a DiscordBotNewsService class has to implement.
    /// </summary>
    public interface IDiscordBotNewsService
    {
        /// <summary>
        /// Initializes the instance of this service with the required missing information it requires to start working.
        /// </summary>
        /// <param name="aTimeout">Http client timeout when trying to get the web news resource.</param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        Task InitAsync(ISwarmBotChannelsService aSwarmBotChannelsService, TimeSpan aTimeout);

        /// <summary>
        /// Sets the number of seconds that the service health check will use to consider if the service is healthy or not
        /// depending on the elapsed time between the last successful http get from news resource until the time of checking the service health.
        /// </summary>
        /// <param name="aSeconds"></param>
        public void SetHealthCheck_Healthy_MaxGetElapsedTime_InSeconds(int aSeconds);

        /// <summary>
        /// Executes one Tick checking if there are news and sending the updates to the designated discord channel if any.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        Task TickExecute(CancellationToken aCancellationToken);

        /// <summary>
        /// Gets a HealthCheck information about this service.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>
        /// <see cref="HealthCheckResult"/> healthy if the last get from the news resource was not empty and it was within the provided <see cref="DiscordBotNewsMasterService.mSucessfulGetRate"/>.</returns>
        HealthCheckResult GetHealthCheck(CancellationToken aCancellationToken = default);

    }
}
