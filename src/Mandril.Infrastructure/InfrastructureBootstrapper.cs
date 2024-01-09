using Mandril.Application;
using Mandril.Infrastructure.Communication.MessageProducer;
using Mandril.Infrastructure.Services;
using MandrilBot;
using MandrilBot.BackgroundServices.NewMemberManager;
using MandrilBot.BackgroundServices.News;
using MandrilBot.HealthChecks;
using MandrilBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TGF.CA.Infrastructure.Communication.RabbitMQ;
using TGF.CA.Infrastructure.Discovery;
using TGF.CA.Infrastructure.Security.Secrets;

namespace Mandril.Infrastructure
{
    /// <summary>
    /// Provides methods for configuring and using the application specific infrastructure layer components.
    /// </summary>
    public static class InfrastructureBootstrapper
    {
        /// <summary>
        /// Configures the necessary infrastructure services for the application.
        /// </summary>
        /// <param name="aWebApplicationBuilder">The web application builder.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static void ConfigureInfrastructure(this WebApplicationBuilder aWebApplicationBuilder)
        {
            aWebApplicationBuilder.Services.AddDiscoveryService(aWebApplicationBuilder.Configuration);
            aWebApplicationBuilder.Services.AddVaultSecretsManager();

            aWebApplicationBuilder.AddCommunicationServices();

            aWebApplicationBuilder.Services.AddSingleton<IMandrilDiscordBot, MandrilDiscordBot>()
                .AddMandrilBotPassiveServices()
                .AddMandrilBotActiveServices()
                .AddMandrilHealthChceckServices();
            aWebApplicationBuilder.Services.AddHostedService<ScToolsBackgroundTasks>();
        }

        /// <summary>
        /// Add Mandril related passive services. Require <see cref="IMandrilDiscordBot"/>.
        /// </summary>
        public static IServiceCollection AddMandrilBotPassiveServices(this IServiceCollection aServiceList)
        {
            //Required by DiscordBotNewsService.
            aServiceList.AddHttpClient()
            .AddSingleton<IDiscordBotNewsService, DiscordBotNewsMasterService>();

            aServiceList.AddSingleton<INewMemberManagementService, NewMemberManagementService>();
            aServiceList.AddHostedService<MandrilBackgroundTasks>();
            aServiceList.AddHostedService<MandrilIntegrationMessageProducer>();

            return aServiceList;

        }

        /// <summary>
        /// Add Mandril related active services. Require <see cref="IMandrilDiscordBot"/>.
        /// </summary>
        public static IServiceCollection AddMandrilBotActiveServices(this IServiceCollection aServiceList)
        {
            aServiceList.AddScoped<IMandrilUsersService, MandrilUsersService>();
            aServiceList.AddScoped<IMandrilChannelsService, MandrilChannelsService>();
            aServiceList.AddScoped<IMandrilMembersService, MandrilMembersService>();
            aServiceList.AddScoped<IMandrilRolesService, MandrilRolesService>();
            aServiceList.AddScoped<IScToolsService, ScToolsService>();

            return aServiceList;

        }

        /// <summary>
        /// Adds Mandril related health checks.
        /// </summary>
        /// <param name="aServiceList"></param>
        /// <returns></returns>
        public static IServiceCollection AddMandrilHealthChceckServices(this IServiceCollection aServiceList)
        {
            aServiceList
                .AddHealthChecks()
                .AddCheck<MandrilBot_HealthCheck>(nameof(MandrilBot_HealthCheck))
                .AddCheck<MandrilAPI_HealthCheck>(nameof(MandrilAPI_HealthCheck))
                .AddCheck<DiscordBotNewsService_HealthCheck>(nameof(DiscordBotNewsService_HealthCheck));
            return aServiceList;
        }

        public static WebApplicationBuilder AddCommunicationServices(this WebApplicationBuilder aWebApplicationBuilder)
        {
            aWebApplicationBuilder.Services.AddServiceBusIntegrationPublisher();
            return aWebApplicationBuilder;
        }


        /// <summary>
        /// Applies the infrastructure configurations to the web application.
        /// </summary>
        /// <param name="aWebApplication">The Web application instance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static void UseInfrastructure(this WebApplication aWebApplication)
        {
            aWebApplication.UseCookiePolicy(new CookiePolicyOptions()//call before any middelware with auth
            {
                MinimumSameSitePolicy = SameSiteMode.Lax
            });
        }

    }
}
