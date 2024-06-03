using SwarmBot.Application;
using SwarmBot.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TGF.Common.Extensions;

namespace SwarmBot.Infrastructure
{
    /// <summary>
    /// BackgroundService to start asynchronously in the background the get data from RSI web.
    /// </summary>
    /// <remarks>Depends on <see cref="IScToolsService"/>.</remarks>
    public class ScToolsBackgroundTasks : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;
        private readonly int _backgroundTick_InSeconds = 60;

        public ScToolsBackgroundTasks(
            IServiceScopeFactory aServiceScopeFactory,
            ILoggerFactory aLoggerFactory,
            IConfiguration aConfiguration)
        {
            _serviceScopeFactory = aServiceScopeFactory;
            _logger = aLoggerFactory.CreateLogger(typeof(ScToolsBackgroundTasks));
            _backgroundTick_InSeconds = aConfiguration.GetValue<int>("ScToolsBackgroundServicesTickInSeconds");
        }

        protected override async Task ExecuteAsync(CancellationToken aStoppingToken)
        {
            try
            {
                while (!aStoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var lScToolsService = scope.ServiceProvider.GetRequiredService<IScToolsService>();
                            await RetryUtility.ExecuteWithRetryAsync(
                                lScToolsService.GetRsiData(),
                                result => !result.IsSuccess,
                                aMaxRetries: 5,
                                aDelayMilliseconds: 2000,
                                aCancellationToken: aStoppingToken);
                        }
                        await Task.Delay(_backgroundTick_InSeconds * 1000, aStoppingToken);
                    }
                    catch (Exception lException)
                    {
                        _logger.LogError("An error occurred during the execution of ScToolsBackgroundTasks: {0}. Stack trace: {1}", lException.ToString(), lException.StackTrace);
                    }
                }
            }
            catch (Exception lException)
            {
                _logger.LogError("RESTART IS REQUIRED: An error occurred during the setup of ScToolsBackgroundTasks: {0}. Stack trace: {1}.", lException.ToString(), lException.StackTrace);
            }

        }
    }
}
