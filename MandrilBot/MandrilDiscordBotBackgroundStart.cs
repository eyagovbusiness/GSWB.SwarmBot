using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MandrilBot
{
    public class MandrilDiscordBotBackgroundStart : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public MandrilDiscordBotBackgroundStart(IServiceProvider aServiceProvider)
        => _serviceProvider = aServiceProvider;
        protected override async Task ExecuteAsync(CancellationToken aStoppingToken)
        {
            var lDiscordBotService = _serviceProvider.GetRequiredService<MandrilDiscordBot>();
            await lDiscordBotService.RunAsync();
            await StopAsync(default);
        }
    }
}
