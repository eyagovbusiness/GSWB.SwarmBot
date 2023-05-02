using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MandrilBot
{
    /// <summary>
    /// Supports health checking of the MandrilDiscordBot service.
    /// </summary>
    public class MandrilBotHealthCheck : IHealthCheck
    {
        private readonly IMandrilDiscordBot _mandrilDiscordBot;

        public MandrilBotHealthCheck(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandrilDiscordBot = aMandrilDiscordBot;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext aContext, CancellationToken aCancellationToken = default)
            => await _mandrilDiscordBot.GetHealthCheck(aCancellationToken);

    }
}
