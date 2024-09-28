using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SwarmBot.Application
{
    /// <summary>
    /// Public interface with the accessible methods of the SwarmBotDiscordBot service.
    /// </summary>
    public interface ISwarmBotDiscordBot
    {
        public event AsyncEventHandler<DiscordClient, GuildMemberUpdateEventArgs> GuildMemberUpdated;
        public event AsyncEventHandler<DiscordClient, GuildRoleCreateEventArgs> GuildRoleCreated;
        public event AsyncEventHandler<DiscordClient, GuildRoleDeleteEventArgs> GuildRoleDeleted;
        public event AsyncEventHandler<DiscordClient, GuildRoleUpdateEventArgs> GuildRoleUpdated;
        public event AsyncEventHandler<DiscordClient, GuildBanAddEventArgs> GuildBanAdded;
        public event AsyncEventHandler<DiscordClient, GuildBanRemoveEventArgs> GuildBanRemoved;
        public event AsyncEventHandler<DiscordClient, GuildCreateEventArgs> GuildAdded;

        /// <summary>
        /// Gets a HealthCheck information about this service by attempting to fetch the target discord guild through the bot client.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>
        /// <see cref="HealthCheckResult"/> healthy if the bot is up and working under 150ms latency, 
        /// degraded in case latency is over 150ms and unhealthy in case the bot is down. </returns>
        Task<HealthCheckResult> GetHealthCheck(CancellationToken aCancellationToken = default);

        /// <summary>
        /// Attempts asynchronously to establish the connection of the internal configured bot with Discord.
        /// </summary>
        /// <returns>awaitable <see cref="Task"/></returns>
        public Task StartAsync();

        /// <summary>
        /// Attempts asynchronously to properly stop the discord bot disposing all the objects and closing all the connections.
        /// </summary>
        /// <returns>awaitable <see cref="Task"/></returns>
        public Task StopAsync();
    }

}