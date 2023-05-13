using MandrilBot.HealthChecks;

namespace MandrilAPI.DI
{
    public static class MandrilBotServicesBuilder_DI
    {
        /// <summary>
        /// Adds all needed health checks for this application.
        /// </summary>
        /// <param name="aWebHostBuilder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder AddHealthChceckServices(this WebApplicationBuilder aWebHostBuilder)
        {
            aWebHostBuilder.Services
                            .AddHealthChecks()
                            .AddCheck<MandrilBot_HealthCheck>(nameof(MandrilBot_HealthCheck))
                            .AddCheck<MandrilAPI_HealthCheck>(nameof(MandrilAPI_HealthCheck))
                            .AddCheck<DiscordBotNewsService_HealthCheck>(nameof(DiscordBotNewsService_HealthCheck));
            return aWebHostBuilder;
        }
    }
}
