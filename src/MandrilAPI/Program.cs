using MandrilAPI.DI;
using MandrilBot;
using MandrilBot.Controllers;
using Microsoft.AspNetCore.Hosting;
using TGF.CA.Application.Setup;
using TGF.CA.Infrastructure.Secrets;

namespace MandrilAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var lWebApplication = WebApplicationAbstraction.CreateCustomWebApplication(
            aWebHostBuilder =>
            {
                aWebHostBuilder.Services.AddVaultSecretsManager(aWebHostBuilder.Configuration);
                aWebHostBuilder.AddHealthChceckServices();
                aWebHostBuilder.AddMandrilBotServices();

                //Implements CQRS pattern, depends on MandrilDiscordBot
                aWebHostBuilder.Services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(Controllers.MandrilController).Assembly);
                });

            });

            lWebApplication.CustomRun();
        }

    }
}