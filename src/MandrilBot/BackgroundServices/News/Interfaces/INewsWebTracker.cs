namespace MandrilBot.BackgroundServices.News.Interfaces
{
    /// <summary>
    /// Public interface to define the minimum behaviours a INewsWebTracker class has to implement.
    /// </summary>
    /// <typeparam name="TNewsMessage"></typeparam>
    internal interface INewsWebTracker<TNewsMessage>
    {
        /// <summary>
        /// Get a List of the last <see cref="TNewsMessage"/> published in the web resource that this news service is tracking.
        /// </summary>
        /// <returns>List of the last <see cref="TNewsMessage"/> published in the web resource that this news service is tracking.</returns>
        Task<List<TNewsMessage>> GetLastMessageListAsync();

        /// <summary>
        /// Gets a list of <see cref="TNewsMessage"/> with the news that were not notified yet in Discord.
        /// </summary>
        /// <returns>List of <see cref="TNewsMessage"/> with the news that were not notified yet in Discord.</returns>
        Task<List<TNewsMessage>> GetUpdatesAsync();

        /// <summary>
        /// Sends message in the designated Diescord news channel for this new service with the given <see cref="TNewsMessage"/>.
        /// </summary>
        /// <param name="aNewsMessage">Source <see cref="TNewsMessage"/>.</param>
        /// <returns>Awaitable <see cref="Task"/>.</returns>
        Task SendMessage(TNewsMessage aNewsMessage);

    }
}
