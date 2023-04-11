using MandrilBot;
using MandrilBot.Controllers;

namespace MandrilAPI
{
    public static class MandrilBotServicesBuilder_DI 
    {
        /// <summary>
        /// Adds all needed MandrilBot related services.
        /// </summary>
        /// <param name="aWebHostBuilder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder AddMandrilBotServices(this WebApplicationBuilder aWebHostBuilder)
        {
            //Singleton MandrilDiscordBot shared by all the services.Services will acess only through the interface protecting the class.
            aWebHostBuilder.Services.AddSingleton<IMandrilDiscordBot, MandrilDiscordBot>();

            //Depends on MandrilDiscordBot, creates the singleton instance and start connection asynchronously in the background.
            aWebHostBuilder.Services.AddHostedService<MandrilDiscordBotBackgroundStart>();

            aWebHostBuilder.Services.AddScoped<IChannelsController, ChannelsController>();
            aWebHostBuilder.Services.AddScoped<IGuildController, GuildController>();
            aWebHostBuilder.Services.AddScoped<IRolesController, RolesController>();
            aWebHostBuilder.Services.AddScoped<IUsersController, UsersController>();

            return aWebHostBuilder;
            
        }
    }
}
