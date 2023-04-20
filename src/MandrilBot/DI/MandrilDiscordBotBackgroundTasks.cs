using MandrilBot.Controllers;
using MandrilBot.News;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;

namespace MandrilBot
{
    /// <summary>
    /// BackgroundService to start asynchronously in the background the connection of the internal configured bot with Discord.
    /// </summary>
    public class MandrilDiscordBotBackgroundTasks : BackgroundService
    {
        private readonly IMandrilDiscordBot _mandrilDiscordBotService;
        private readonly IDiscordBotNewsService _discordBotNewsService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;

        public MandrilDiscordBotBackgroundTasks(
            IMandrilDiscordBot aMandrilDiscordBot,
            IDiscordBotNewsService aDiscordBotNewsService,
            IServiceScopeFactory aServiceScopeFactory,
            ILoggerFactory aLoggerFactory)
        {
            _mandrilDiscordBotService = aMandrilDiscordBot;
            _discordBotNewsService = aDiscordBotNewsService;
            _serviceScopeFactory = aServiceScopeFactory;
            _logger = aLoggerFactory.CreateLogger(typeof(MandrilDiscordBotBackgroundTasks));
        }

        protected override async Task ExecuteAsync(CancellationToken aStoppingToken)
        {
            try
            {
                await _mandrilDiscordBotService.StartAsync();

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var lDiscordChannelsControllerService = scope.ServiceProvider.GetRequiredService<IChannelsController>();
                    await _discordBotNewsService.InitAsync(lDiscordChannelsControllerService);
                }
                while (!aStoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(10000);
                    await _discordBotNewsService.TickExecute(aStoppingToken);
                }
            }
            catch (Exception lException)
            {
                _logger.LogError("An error occurred during the inital execution of MandrilDiscordBotBackgroundTasks: ", lException.ToString());
            }

        }
    }
}
