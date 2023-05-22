﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using MandrilBot.BackgroundServices.NewMemberManager;
using MandrilBot.Commands;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace MandrilBot
{
    /// <summary>
    /// Class designed as a service to provide comminication with an internal Discord bot in this service, with functionality accessible through the public interface.
    /// </summary>
    public partial class MandrilDiscordBot : IMandrilDiscordBot
    {
        internal static readonly byte _maxDegreeOfParallelism = Convert.ToByte(Math.Ceiling(Environment.ProcessorCount * 0.75));

        /// <summary>
        /// Gets a HealthCheck information about this service by attempting to fetch the target discord guild through the bot client.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>
        /// <see cref="HealthCheckResult"/> healthy if the bot is up and working under 150ms latency, 
        /// dergraded in case latency is over 150ms and unhealthy in case the bot is down. </returns>
        public async Task<HealthCheckResult> GetHealthCheck(CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            try
            {
                var lGuilPreview = await Client.GetGuildPreviewAsync(BotConfiguration.DiscordTargetGuildId);
                if (lGuilPreview?.ApproximateMemberCount != null && lGuilPreview?.ApproximateMemberCount > 0)
                {
                    if (Client.Ping < 300)
                        return HealthCheckResult.Healthy(string.Format("The service is healthy. ({0}ms) ping", Client.Ping));
                    else
                        return HealthCheckResult.Degraded(string.Format("The service's health is degraded due to high latency. ({0}ms) ping", Client.Ping));
                }
            }
            catch (Exception lException)
            {
                return HealthCheckResult.Unhealthy("The service is down at the moment, an exception was thrown fetching the guild preview.", lException);
            }
            return HealthCheckResult.Unhealthy("The service is down at th moment, could not fetch guild preview.");

        }

        /// <summary>
        /// Attempts asynchronously to establish the connection of the internal configured bot with Discord.
        /// </summary>
        /// <returns>awaitable <see cref="Task"/></returns>
        public async Task StartAsync()
        {
            try
            {
                await SetBotConfigurationAsync();
                var config = new DiscordConfiguration
                {
                    UseRelativeRatelimit= true,
                    LoggerFactory = _loggerFactory,
                    Token = BotConfiguration.MandrilBotToken,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    Intents = DiscordIntents.MessageContents
                              | DiscordIntents.GuildMessageReactions
                              | DiscordIntents.GuildMessages
                              | DiscordIntents.Guilds
                              | DiscordIntents.GuildVoiceStates
                              | DiscordIntents.GuildPresences
                };

                Client = new DiscordClient(config);

                var lCommandsConfig = new CommandsNextConfiguration
                {
                    StringPrefixes = new string[] { BotConfiguration.BotCommandPrefix },
                    EnableDms = false,
                    EnableMentionPrefix = true,
                    Services = GetCommandsServiceProvider(), 
                };

                Commands = Client.UseCommandsNext(lCommandsConfig);
                Commands.RegisterCommands<BotAdminCommands>();
                Commands.RegisterCommands<BotCommands>();

                await Client.ConnectAsync();

            }
            catch (Exception lException)
            {
                _loggerFactory.CreateLogger(typeof(MandrilDiscordBot)).LogError("An error occurred while attempting to start the Discord bot client: {0}. Stack trace: {1} ", lException.ToString(), lException.StackTrace);
            }
        }

        private async Task SetBotConfigurationAsync()
        {
            BotConfiguration = await _secretsManager.Get<BotConfig>("mandrilbot");
            BotConfiguration.BotCommandPrefix = _configuration.GetValue<string>("BotCommandPrefix");
        }

        private IServiceProvider GetCommandsServiceProvider()
        {
            var lNewConfiguration = new ConfigurationBuilder()
                .AddConfiguration(_configuration)
                .Build();

            var lServices = new ServiceCollection();
            lServices.AddSingleton<IMandrilDiscordBot>(this);
            lServices.AddScoped<IMembersController, MembersController>();
            lServices.AddScoped<INewMemberManagementService, NewMemberManagementService>();
            lServices.AddSingleton<IConfiguration>(lNewConfiguration); // Register lNewConfiguration as IConfiguration
            return lServices.BuildServiceProvider();
        }

    }
}