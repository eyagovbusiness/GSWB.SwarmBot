using Mandril.API;
using Mandril.Application;
using Mandril.Infrastructure;

WebApplicationBuilder lMandrilApplicationBuilder = WebApplication.CreateBuilder();

lMandrilApplicationBuilder.ConfigureInfrastructure();
lMandrilApplicationBuilder.Services.RegisterApplicationServices();
lMandrilApplicationBuilder.ConfigurePresentation();

var lMandrilWebApplication = lMandrilApplicationBuilder.Build();

lMandrilWebApplication.UseInfrastructure();
lMandrilWebApplication.UsePresentation();

await lMandrilWebApplication.RunAsync();