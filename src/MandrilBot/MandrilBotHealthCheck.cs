using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MandrilBot
{
    /// <summary>
    /// Supports health checking of the MandrilDiscordBot service.
    /// </summary>
    public class MandrilBotHealthCheck : IHealthCheck
    {
        private readonly IServiceProvider _serviceProvider;


        public MandrilBotHealthCheck(IServiceProvider aServiceProvider)
        {
            _serviceProvider = aServiceProvider;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext aContext, CancellationToken aCancellationToken = default)
        {
            var lService = _serviceProvider.GetRequiredService<IMandrilDiscordBot>();
            return await lService.GetHealthCheck(aCancellationToken);

        }
    }
}
