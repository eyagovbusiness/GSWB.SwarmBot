using SwarmBot.Application;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SwarmBot.HealthChecks
{
    /// <summary>
    /// Supports health checking of the SwarmBotDiscordBot service.
    /// </summary>
    public class SwarmBot_HealthCheck : IHealthCheck
    {
        private readonly ISwarmBotDiscordBot _SwarmBotDiscordBot;

        public SwarmBot_HealthCheck(ISwarmBotDiscordBot aSwarmBotDiscordBot)
            => _SwarmBotDiscordBot = aSwarmBotDiscordBot;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext aContext, CancellationToken aCancellationToken = default)
            => await _SwarmBotDiscordBot.GetHealthCheck(aCancellationToken);

    }
}
