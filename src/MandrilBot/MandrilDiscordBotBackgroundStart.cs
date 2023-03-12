using AngleSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using static MandrilBot.News.DiscordBotSCNews;

namespace MandrilBot
{
    /// <summary>
    /// BackgroundService to start asynchronously in the background the connection of the internal configured bot with Discord.
    /// </summary>
    public class MandrilDiscordBotBackgroundStart : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private Timer _clock;
        private DevTrackerNews _devTrackerNews;

        public MandrilDiscordBotBackgroundStart(IServiceProvider aServiceProvider, Microsoft.Extensions.Configuration.IConfiguration aConfiguration)
        {
            _serviceProvider = aServiceProvider;
            _configuration = aConfiguration;
        }

        protected override async Task ExecuteAsync(CancellationToken aStoppingToken)
        {
            var lDiscordBotService = _serviceProvider.GetRequiredService<IMandrilDiscordBot>();
            await lDiscordBotService.StartAsync();

            //_configuration.get

            _clock = new Timer(ExecuteTickTasks, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

        }

        private void ExecuteTickTasks(object state)
        {

        }
    }
}
