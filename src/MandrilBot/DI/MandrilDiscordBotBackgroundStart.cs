using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MandrilBot
{
    /// <summary>
    /// BackgroundService to start asynchronously in the background the connection of the internal configured bot with Discord.
    /// </summary>
    public class MandrilDiscordBotBackgroundStart : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public MandrilDiscordBotBackgroundStart(IServiceProvider aServiceProvider)
            => _serviceProvider = aServiceProvider;
        protected override async Task ExecuteAsync(CancellationToken aStoppingToken)
        {
            var lDiscordBotService = _serviceProvider.GetRequiredService<IMandrilDiscordBot>();
            await lDiscordBotService.StartAsync();
            await StopAsync(default);
        }
    }
}
