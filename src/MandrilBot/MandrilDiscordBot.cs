using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Mandril.Application;
using MandrilBot.BackgroundServices.NewMemberManager;
using MandrilBot.Commands;
using MandrilBot.Configuration;
using MandrilBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using TGF.CA.Application;

namespace MandrilBot
{
    /// <summary>
    /// Class designed as a service to provide comminication with an internal Discord bot in this service, with functionality accessible through the public interface.
    /// </summary>
    public partial class MandrilDiscordBot : IMandrilDiscordBot
    {
        public static readonly byte _maxDegreeOfParallelism = Convert.ToByte(Math.Ceiling(Environment.ProcessorCount * 0.75));
        private bool mIsAutoBanBotsEnabled = true;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        private readonly ISecretsManager _secretsManager;
        internal BotConfig BotConfiguration { get; private set; }

        internal DiscordClient Client { get; private set; }
        internal CommandsNextExtension Commands { get; private set; }

        public MandrilDiscordBot(ILoggerFactory aLoggerFactory, ISecretsManager aSecretsManager, IConfiguration aConfiguration)
        {
            _loggerFactory = aLoggerFactory;
            _secretsManager = aSecretsManager;
            _configuration = aConfiguration;
        }

        #region IMandrilDiscordBot

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

                ConfigureDiscordBotClient();
                ConfigureDiscordBotCommands();

                await Client.ConnectAsync();
            }
            catch (Exception lException)
            {
                _loggerFactory.CreateLogger(typeof(MandrilDiscordBot)).LogError("An error occurred while attempting to start the Discord bot client: {0}. Stack trace: {1} ", lException.ToString(), lException.StackTrace);
            }
        }

        /// <summary>
        /// Unsubscribe the event handler that do ban new bots joined temporarily.
        /// </summary>
        /// <param name="aMinutesAllowed">Number of minutes the new bots will be allowed to join the guild.</param>
        /// <returns>True if the status changed from not allowed to allowed, false if nothing changed because it was allowed already at this time.</returns>
        public async Task<bool> AllowTemporarilyJoinNewBots(int aMinutesAllowed)
        {
            if (!mIsAutoBanBotsEnabled)
                return mIsAutoBanBotsEnabled;
            mIsAutoBanBotsEnabled = !mIsAutoBanBotsEnabled;
            await Task.Delay(TimeSpan.FromMinutes(aMinutesAllowed));
            mIsAutoBanBotsEnabled = !mIsAutoBanBotsEnabled;
            return mIsAutoBanBotsEnabled;
        }


        #endregion

        #region BotConfiguration

        private void ConfigureDiscordBotClient()
        {
            var config = new DiscordConfiguration
            {
                UseRelativeRatelimit = true,
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
                          | DiscordIntents.GuildMembers
            };

            Client = new DiscordClient(config);
            //By default the bot will be banning new bots unless it is temporary disabled via admin commands.
            Client.GuildMemberAdded += BotOnGuildMemberAdded;
        }

        private async Task SetBotConfigurationAsync()
        {
            BotConfiguration = await _secretsManager.Get<BotConfig>("mandrilbot");
            BotConfiguration.BotCommandPrefix = _configuration.GetValue<string>("BotCommandPrefix");
        }

        private void ConfigureDiscordBotCommands()
        {
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
        }

        private IServiceProvider GetCommandsServiceProvider()
        {
            var lNewConfiguration = new ConfigurationBuilder()
                .AddConfiguration(_configuration)
                .Build();

            var lServices = new ServiceCollection();
            lServices.AddSingleton<IMandrilDiscordBot>(this);
            lServices.AddScoped<IMandrilMembersService, MandrilMembersService>();
            lServices.AddScoped<INewMemberManagementService, NewMemberManagementService>();
            lServices.AddSingleton<IConfiguration>(lNewConfiguration); // Register lNewConfiguration as IConfiguration
            return lServices.BuildServiceProvider();
        }

        #endregion

        #region BotEvents

        /// <summary>
        /// Ban every new bot that joins the server unless it is allowed for short time by an admin using the admin command "?open-bots".
        /// </summary>
        private async Task BotOnGuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            if (mIsAutoBanBotsEnabled && e.Guild.Id == BotConfiguration.DiscordTargetGuildId && e.Member.IsBot)
            {
                await e.Member.BanAsync(reason: "Bot detected.");
                Console.WriteLine($"Banned bot: {e.Member.Username}#{e.Member.Discriminator}.");
            }
        }

        #endregion

    }
}