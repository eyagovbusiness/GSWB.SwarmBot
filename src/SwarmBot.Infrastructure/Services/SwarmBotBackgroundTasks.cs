using SwarmBot.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SwarmBot.Infrastructure
{
    /// <summary>
    /// BackgroundService to start asynchronously in the background the connection of the internal configured bot with Discord.
    /// </summary>
    /// <remarks>Depends on <see cref="ISwarmBotDiscordBot"/>, <see cref="IDiscordBotNewsService"/> and <see cref="INewMemberManagementService"/>.</remarks>
    public class SwarmBotBackgroundTasks : BackgroundService
    {
        private readonly ISwarmBotDiscordBot _SwarmBotDiscordBotService;
        private readonly INewMemberManagementService _newMemberManagerService;
        private readonly IDiscordBotNewsService _discordBotNewsService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;
        private readonly int _backgroundTick_InSeconds = 10;

        public SwarmBotBackgroundTasks(
            ISwarmBotDiscordBot aSwarmBotDiscordBot,
            INewMemberManagementService aNewMemberManagerService,
            IDiscordBotNewsService aDiscordBotNewsService,
            IServiceScopeFactory aServiceScopeFactory,
            ILoggerFactory aLoggerFactory,
            IConfiguration aConfiguration)
        {
            _SwarmBotDiscordBotService = aSwarmBotDiscordBot;
            _newMemberManagerService = aNewMemberManagerService;
            _discordBotNewsService = aDiscordBotNewsService;
            _serviceScopeFactory = aServiceScopeFactory;
            _logger = aLoggerFactory.CreateLogger(typeof(SwarmBotBackgroundTasks));
            _backgroundTick_InSeconds = aConfiguration.GetValue<int>("BackgroundServicesTickInSeconds");
        }

        protected override async Task ExecuteAsync(CancellationToken aStoppingToken)
        {
            try
            {
                await _SwarmBotDiscordBotService.StartAsync();

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var lDiscordChannelsControllerService = scope.ServiceProvider.GetRequiredService<ISwarmBotChannelsService>();
                    await _discordBotNewsService.InitAsync(lDiscordChannelsControllerService, new TimeSpan(0, 0, _backgroundTick_InSeconds));
                    _discordBotNewsService.SetHealthCheck_Healthy_MaxGetElapsedTime_InSeconds(_backgroundTick_InSeconds * 3);//3 failure threshold
                }

                while (!aStoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var lTimeNow = DateTimeOffset.Now.TimeOfDay;
                        if (lTimeNow >= new TimeSpan(5, 0, 0) && lTimeNow < new TimeSpan(5, 0, _backgroundTick_InSeconds * 2))//execute daily task at 5 AM UTC(consider moving to System.Threading.Timer) 
                            await _newMemberManagerService.DoDailyTaskAsync(aStoppingToken);

                        await Task.Delay(_backgroundTick_InSeconds * 1000, aStoppingToken);
                        await _discordBotNewsService.TickExecute(aStoppingToken);
                    }
                    catch (Exception lException)
                    {
                        _logger.LogError("An error occurred during the execution of SwarmBotDiscordBotBackgroundTasks: {0}. Stack trace: {1}", lException.ToString(), lException.StackTrace);
                    }

                }
            }
            catch (Exception lException)
            {
                _logger.LogError("RESTART IS REQUIRED: An error occurred during the setup of SwarmBotDiscordBotBackgroundTasks: {0}. Stack trace: {1}.", lException.ToString(), lException.StackTrace);
            }

        }
    }
}
