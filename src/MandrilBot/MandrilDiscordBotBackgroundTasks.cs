using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MandrilBot.Configuration;
using Microsoft.Extensions.Logging;
using MandrilBot.News;

namespace MandrilBot
{
    /// <summary>
    /// BackgroundService to start asynchronously in the background the connection of the internal configured bot with Discord.
    /// </summary>
    public class MandrilDiscordBotBackgroundTasks : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public MandrilDiscordBotBackgroundTasks(
            IServiceProvider aServiceProvider, 
            ILoggerFactory aLoggerFactory)
        {
            _serviceProvider = aServiceProvider;
            _logger = aLoggerFactory.CreateLogger(typeof(MandrilDiscordBotBackgroundTasks));
        }

        protected override async Task ExecuteAsync(CancellationToken aStoppingToken)
        {
            try
            {
                var lDiscordBotService = _serviceProvider.GetRequiredService<IMandrilDiscordBot>();
                var lDiscordBotNewsService = _serviceProvider.GetRequiredService<IDiscordBotNewsService>();

                await lDiscordBotService.StartAsync();
                await lDiscordBotNewsService.InitAsync(lDiscordBotService);

                while (!aStoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(10000);
                    await lDiscordBotNewsService.TickExecute(aStoppingToken);
                }
            }
            catch (Exception lException)
            {
                _logger.LogError("An error occurred during the inital execution of MandrilDiscordBotBackgroundTasks: ", lException.ToString());
            }

        }

    }
}
