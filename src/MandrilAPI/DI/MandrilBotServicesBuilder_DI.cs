using MandrilBot;
using MandrilBot.BackgroundServices.NewMemberManager;
using MandrilBot.BackgroundServices.News;
using MandrilBot.BackgroundServices.News.Interfaces;
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
            //Depends on MandrilDiscordBot: add Star Citizen news service
            aWebHostBuilder.Services.AddSingleton<IDiscordBotNewsService, DiscordBotNewsMasterService>();
            //Add new guild members manager service
            aWebHostBuilder.Services.AddSingleton<INewMemberManagementService, NewMemberManagementService>();
            //Depends on MandrilDiscordBot and DiscordBotNewsService: create the singleton instance and start connection asynchronously in the background. 
            aWebHostBuilder.Services.AddHostedService<MandrilDiscordBotBackgroundTasks>();
            //Depends on MandrilDiscordBot: add controllers
            aWebHostBuilder.Services.AddScoped<IUsersController, UsersController>();
            aWebHostBuilder.Services.AddScoped<IGuildController, GuildController>();
            aWebHostBuilder.Services.AddScoped<IChannelsController, ChannelsController>();
            aWebHostBuilder.Services.AddScoped<IMembersController, MembersController>();
            aWebHostBuilder.Services.AddScoped<IRolesController, RolesController>();

            return aWebHostBuilder;

        }
    }
}
