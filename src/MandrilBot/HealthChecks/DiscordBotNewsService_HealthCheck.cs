using MandrilBot.News;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandrilBot.HealthChecks
{
    public class DiscordBotNewsService_HealthCheck : IHealthCheck
    {
        private readonly IDiscordBotNewsService _discordBotNewsService;

        public DiscordBotNewsService_HealthCheck(IDiscordBotNewsService aDiscordBotNewsService)
            => _discordBotNewsService = aDiscordBotNewsService;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext aContext, CancellationToken aCancellationToken = default)
            => await Task.FromResult(_discordBotNewsService.GetHealthCheck(aCancellationToken));

    }

}
