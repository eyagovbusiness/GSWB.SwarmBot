using DSharpPlus;
using DSharpPlus.CommandsNext;
using MandrilBot.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace MandrilBot
{
    /// <summary>
    /// Class designed as a service to provide comminication with an internal Discord bot in this service, with functionality accessible through the public interface.
    /// </summary>
    public partial class MandrilDiscordBot : IMandrilDiscordBot
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        internal BotConfigJson _botConfiguration;

        internal DiscordClient Client { get; private set; }
        internal CommandsNextExtension Commands { get; private set; }

        public MandrilDiscordBot(IConfiguration aConfiguration, ILoggerFactory aLoggerFactory)
        {
            _logger = aLoggerFactory.CreateLogger(typeof(MandrilDiscordBot));
            _botConfiguration = aConfiguration.Get<BotConfigJson>();
            _loggerFactory = aLoggerFactory;
        }

    }

    /// <summary>
    /// Public interfacewith the accessible methods of the MandrilDiscordBot service.
    /// </summary>
    public interface IMandrilDiscordBot
    {
        /// <summary>
        /// Gets a HealthCheck information about this service by attempting to fetch the target discord guild through the bot client.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>
        /// <see cref="HealthCheckResult"/> healthy if the bot is up and working under 150ms latency, 
        /// dergraded in case latency is over 150ms and unhealthy in case the bot is down. </returns>
        Task<HealthCheckResult> GetHealthCheck(CancellationToken aCancellationToken = default);

        /// <summary>
        /// Attempts asynchronously to establish the connection of the internal configured bot with Discord.
        /// </summary>
        /// <returns>awaitable <see cref="Task"/></returns>
        public Task StartAsync();
    }

}