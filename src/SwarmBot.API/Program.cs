using Common.Domain;
using SwarmBot.API;
using SwarmBot.Application;
using SwarmBot.Infrastructure;

WebApplicationBuilder lSwarmBotApplicationBuilder = WebApplication.CreateBuilder(args);

lSwarmBotApplicationBuilder.ConfigureCommonDomain();
lSwarmBotApplicationBuilder.ConfigureInfrastructure();
lSwarmBotApplicationBuilder.Services.RegisterApplicationServices();
lSwarmBotApplicationBuilder.ConfigurePresentation();

var lSwarmBotWebApplication = lSwarmBotApplicationBuilder.Build();

lSwarmBotWebApplication.UseInfrastructure();
lSwarmBotWebApplication.UsePresentation();

await lSwarmBotWebApplication.RunAsync();