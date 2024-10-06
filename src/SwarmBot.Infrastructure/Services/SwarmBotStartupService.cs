using Microsoft.Extensions.Hosting;
using SwarmBot.Application;

namespace SwarmBot.Infrastructure.Services
{
    public class SwarmBotStartupService(ISwarmBotDiscordBot aSwarmBotDiscordBotService) : IHostedService
    {
        public async Task StartAsync(CancellationToken aStoppingToken)
        => await aSwarmBotDiscordBotService.StartAsync();

        public async Task StopAsync(CancellationToken aStoppingToken)
        => await aSwarmBotDiscordBotService.StopAsync();

    }
    ///AS BackgoundService
    //public class SwarmBotStartupService : BackgroundService
    //{
    //    private readonly ISwarmBotDiscordBot _swarmBotDiscordBotService;

    //    public SwarmBotStartupService(ISwarmBotDiscordBot swarmBotDiscordBotService)
    //    {
    //        _swarmBotDiscordBotService = swarmBotDiscordBotService;
    //    }

    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        await _swarmBotDiscordBotService.StartAsync();
    //    }

    //    public override async Task StopAsync(CancellationToken stoppingToken)
    //    {
    //        await _swarmBotDiscordBotService.StopAsync();
    //        await base.StopAsync(stoppingToken);
    //    }
    //}
}
