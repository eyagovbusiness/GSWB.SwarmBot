using Common.Domain;
using SwarmBot.API;
using SwarmBot.Application;
using SwarmBot.Infrastructure;

WebApplicationBuilder lSwarmBotApplicationBuilder = WebApplication.CreateBuilder(args);

lSwarmBotApplicationBuilder.ConfigureCommonDomain();
await lSwarmBotApplicationBuilder.ConfigureInfrastructure();
lSwarmBotApplicationBuilder.Services.RegisterApplicationServices();
lSwarmBotApplicationBuilder.ConfigurePresentation();

var lSwarmBotWebApplication = lSwarmBotApplicationBuilder.Build();

await lSwarmBotWebApplication.UseInfrastructure();
lSwarmBotWebApplication.UsePresentation();

await lSwarmBotWebApplication.RunAsync();