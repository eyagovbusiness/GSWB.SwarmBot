using TGF.CA.Presentation;
using Mandril.Application;
using Mandril.Infrastructure;
using Mandril.API;
using System.Reflection;


var lXmlDocFileName = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
var lXmlDocFilePath = Path.Combine(AppContext.BaseDirectory, lXmlDocFileName);

WebApplicationBuilder lApplicationBuilder = WebApplication.CreateBuilder();

lApplicationBuilder.ConfigureInfrastructure();
lApplicationBuilder.Services.RegisterApplicationServices();
lApplicationBuilder.ConfigureDefaultPresentation(
    new List<string> { lXmlDocFilePath }, 
    aBaseSwaggerPath: "mandril-ms", 
    aScanMarkerList: typeof(Errors));


var lWebApplication = lApplicationBuilder.Build();

lWebApplication.UseInfrastructure();
lWebApplication.UseDefaultPresentationConfigurations(aUseIdentity: false, aUseCORS:false);

await lWebApplication.RunAsync();