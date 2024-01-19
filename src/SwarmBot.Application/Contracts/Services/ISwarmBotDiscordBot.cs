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

        /// <summary>
        /// Unsubscribe the event handler that do ban new bots joined temporarily.
        /// </summary>
        /// <param name="aMinutesAllowed">Number of minutes the new bots will be allowed to join the guild.</param>
        /// <returns>True if the status changed from not allowed to allowed, false if nothing changed because it was allowed already at this time.</returns>
        public Task<bool> AllowTemporarilyJoinNewBots(int aMinutesAllowed);

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
    }

}