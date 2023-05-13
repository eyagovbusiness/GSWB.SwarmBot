using MandrilBot.Controllers;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MandrilBot.News
{
    /// <summary>
    /// Public interface with the accessible methods of any DiscordBotSCNews service.
    /// </summary>
    public interface IDiscordBotNewsService
    {

        /// <summary>
        /// Gets a HealthCheck information about this service.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>
        /// <see cref="HealthCheckResult"/> healthy if the last get from the news resource was not empty and it was within the provided <see cref="DiscordBotNewsService.mSucessfulGetRate"/>.</returns>
        HealthCheckResult GetHealthCheck(CancellationToken aCancellationToken = default);

        /// <summary>
        /// Sets the number of seconds that the service healtcheck will use to consider if the service is healthy or not
        /// depeding on the elapsed time between the last successful http get from news resource until the time of checking the service health.
        /// </summary>
        /// <param name="aSeconds"></param>
        public void SetHealthCheck_Healthy_MaxGetElapsedTime_InSeconds(int aSeconds);

        /// <summary>
        /// Initializes the instance of this service with the requiered missin information it requieres to start working.
        /// </summary>
        /// <param name="aHttpClient"><see cref="HttpClient"/> that will be used to get the news in every Tick.</param>
        /// <param name="aMandrilDiscordBot">Reference to the Discord bot that will send the notification messages.</param>
        /// <param name="aChannelName">Name of the <see cref="DiscordChannel"/> to send the notification messages when available.</param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        Task InitAsync(IChannelsController aDiscordChannelsControllerService);

        /// <summary>
        /// Executes one Tick checking if there are news and sending the updates to the designated discord channel if any.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        Task TickExecute(CancellationToken aCancellationToken);
    }
}
