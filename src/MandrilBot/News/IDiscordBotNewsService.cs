using MandrilBot.Controllers;

namespace MandrilBot.News
{
    /// <summary>
    /// Public interface with the accessible methods of any DiscordBotSCNews service.
    /// </summary>
    public interface IDiscordBotNewsService
    {

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
