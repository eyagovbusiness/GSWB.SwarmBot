using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using static MandrilBot.News.DiscordBotSCNews;
using MandrilBot.Configuration;
using Microsoft.Extensions.Logging;

namespace MandrilBot
{
    /// <summary>
    /// BackgroundService to start asynchronously in the background the connection of the internal configured bot with Discord.
    /// </summary>
    public class MandrilDiscordBotBackgroundTasks : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private DevTrackerNews _devTrackerNews;
        private HttpClient _httpClient;

        public MandrilDiscordBotBackgroundTasks(
            IServiceProvider aServiceProvider, 
            IConfiguration aConfiguration,
            HttpClient aHttpClient,
            ILoggerFactory aLoggerFactory)
        {
            _serviceProvider = aServiceProvider;
            _configuration = aConfiguration;
            _httpClient = aHttpClient;
            _logger = aLoggerFactory.CreateLogger(typeof(MandrilDiscordBotBackgroundTasks));
        }

        protected override async Task ExecuteAsync(CancellationToken aStoppingToken)
        {
            try
            {
                var lDiscordBotService = _serviceProvider.GetRequiredService<IMandrilDiscordBot>();
                await lDiscordBotService.StartAsync();

                InitTasks(_configuration, lDiscordBotService);

                while (!aStoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(10000);
                    await ExecuteTickTasksAsync(aStoppingToken);
                }
            }
            catch (Exception lException)
            {
                _logger.LogError("An error occurred during the inital execution of MandrilDiscordBotBackgroundTasks: ", lException.ToString());
            }

        }

        private async void InitTasks(IConfiguration aConfiguration, IMandrilDiscordBot aMandrilDiscordBot)
        {
            try
            {
                var lBotNewsConfig = new BotNewsConfig();
                aConfiguration.Bind("BotNews",lBotNewsConfig);
                _httpClient.BaseAddress = new Uri(lBotNewsConfig.BaseResourceAddress);
                _devTrackerNews = new DevTrackerNews(lBotNewsConfig);

                await _devTrackerNews.InitAsync(_httpClient, aMandrilDiscordBot, lBotNewsConfig.DevTracker.DiscordChannel);
            }
            catch (Exception lException)
            {
                _logger.LogError("An error occurred during the MandrilDiscordBotBackgroundTasks.InitTasks(): ", lException.ToString());
            }

        }

        private async Task ExecuteTickTasksAsync(CancellationToken aStoppingToken)
        {
            try
            {
                await _devTrackerNews.TickExecute(aStoppingToken);
            }
            catch (Exception lException)
            {
                _logger.LogError("An error occurred during the MandrilDiscordBotBackgroundTasks.ExecuteTickTasksAsync(): ", lException.ToString());
            }
        }

    }
}
