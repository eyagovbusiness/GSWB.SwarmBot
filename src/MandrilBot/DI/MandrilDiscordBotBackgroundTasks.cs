using DSharpPlus.Entities;
using MandrilBot.BackgroundServices.NewMemberManager;
using MandrilBot.BackgroundServices.News.Interfaces;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TGF.Common.Extensions;
using TGF.Common.Extensions.Serialization;

namespace MandrilBot
{
    /// <summary>
    /// BackgroundService to start asynchronously in the background the connection of the internal configured bot with Discord.
    /// </summary>
    public class MandrilDiscordBotBackgroundTasks : BackgroundService
    {
        private readonly IMandrilDiscordBot _mandrilDiscordBotService;
        private readonly INewMemberManagementService _newMemberManagerService;
        private readonly IDiscordBotNewsService _discordBotNewsService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;

        private readonly int _backgroundTick_InSeconds = 10; 

        public MandrilDiscordBotBackgroundTasks(
            IMandrilDiscordBot aMandrilDiscordBot,
            INewMemberManagementService aNewMemberManagerService,
            IDiscordBotNewsService aDiscordBotNewsService,
            IServiceScopeFactory aServiceScopeFactory,
            ILoggerFactory aLoggerFactory)
        {
            _mandrilDiscordBotService = aMandrilDiscordBot;
            _newMemberManagerService = aNewMemberManagerService;
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
                    _discordBotNewsService.SetHealthCheck_Healthy_MaxGetElapsedTime_InSeconds(_backgroundTick_InSeconds*3);//3 failure threshold
                }

                while (!aStoppingToken.IsCancellationRequested)
                {
                    if (DateTimeOffset.UtcNow.TimeOfDay == new TimeSpan(5, 0, 0)) //true every day at 5 AM UTC
                        await _newMemberManagerService.DoDailyTaskAsync(aStoppingToken);

                    await Task.Delay(_backgroundTick_InSeconds * 1000, aStoppingToken);
                    await _discordBotNewsService.TickExecute(aStoppingToken);
                }
            }
            catch (Exception lException)
            {
                _logger.LogError("An error occurred during the execution of MandrilDiscordBotBackgroundTasks: {0}. Stack trace: {1}", lException.ToString(), lException.StackTrace);
            }

        }
    }
}
