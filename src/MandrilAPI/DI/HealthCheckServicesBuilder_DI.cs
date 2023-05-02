using MandrilBot;

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
                            .AddCheck<MandrilBotHealthCheck>(nameof(MandrilBotHealthCheck))
                            .AddCheck<MandrilAPIGeneralHealthCheck>(nameof(MandrilAPIGeneralHealthCheck));
            return aWebHostBuilder;
        }
    }
}
