using MandrilAPI.Configuration;
using TheGoodFramework.CA.Application;

namespace MandrilAPI
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var lWebApplication = await WebApplicationAbstraction.CreateCustomWebApplicationAsync(
            async(aWebHostBuilderAction) =>
            {
                await aWebHostBuilderAction.SetDiscordBotConfiguration();
                aWebHostBuilderAction.Services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(Controllers.MandrilController).Assembly);
                });
            });

            lWebApplication.CustomRun();
        }
    }
}