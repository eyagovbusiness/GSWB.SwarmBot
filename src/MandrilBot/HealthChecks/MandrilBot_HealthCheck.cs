using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MandrilBot.HealthChecks
{
    /// <summary>
    /// Supports health checking of the MandrilDiscordBot service.
    /// </summary>
    public class MandrilBot_HealthCheck : IHealthCheck
    {
        private readonly IMandrilDiscordBot _mandrilDiscordBot;

        public MandrilBot_HealthCheck(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandrilDiscordBot = aMandrilDiscordBot;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext aContext, CancellationToken aCancellationToken = default)
            => await _mandrilDiscordBot.GetHealthCheck(aCancellationToken);

    }
}
