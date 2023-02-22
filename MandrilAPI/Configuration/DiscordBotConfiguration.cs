using MandrilBot;

namespace MandrilAPI.Configuration
{
    public static class DiscordBotConfiguration
    {
        public static async Task SetDiscordBotConfiguration(this WebApplicationBuilder aWebApplication)
        {
            var lBotConfig = aWebApplication.Configuration.Get<BotConfigJson>();
            aWebApplication.Services.AddSingleton(await MandrilDiscordBot.CreateAsync(lBotConfig));
        }
    }
}
