using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MandrilBot
{
    /// <summary>
    /// Supports health checking of the MandrilDiscordBot service.
    /// </summary>
    public class MandrilBotHealthCheck : IHealthCheck
    {
        //private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceProvider _serviceProvider;
        

        public MandrilBotHealthCheck(/*IHttpClientFactory aHttpClientFactory, */IServiceProvider aServiceProvider) 
        {
            //_httpClientFactory = aHttpClientFactory;
            _serviceProvider = aServiceProvider;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext aContext, CancellationToken aCancellationToken = default)
        {
            //HttpClient lClient = _httpClientFactory.CreateClient();
            var lService = _serviceProvider.GetRequiredService<IMandrilDiscordBot>();
            return await lService.GetHealthCheck(aCancellationToken);

        }
    }
}
