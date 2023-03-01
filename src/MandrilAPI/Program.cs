using MandrilBot;
using TGF.CA.Application;

namespace MandrilAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var lWebApplication = WebApplicationAbstraction.CreateCustomWebApplication(
            aWebHostBuilder =>
            {
                aWebHostBuilder.Services
                                .AddHealthChecks()
                                .AddCheck<MandrilBotHealthCheck>(nameof(MandrilBotHealthCheck));

                //Singleton MandrilDiscordBot shared by all the services.Services will acess only through the interface protecting the class.
                aWebHostBuilder.Services.AddSingleton<IMandrilDiscordBot, MandrilDiscordBot>();
                //Depends on MandrilDiscordBot, creates the singleton instance and start connection asynchronously in the background.
                aWebHostBuilder.Services.AddHostedService<MandrilDiscordBotBackgroundStart>();
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