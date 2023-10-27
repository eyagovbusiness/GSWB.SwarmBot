using DSharpPlus;
using DSharpPlus.AsyncEvents;
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
    /// Class designed as a service to provide communication with an internal Discord bot in this service, with functionality accessible through the public interface.
    /// </summary>
    public partial class MandrilDiscordBot : IMandrilDiscordBot, IDisposable
    {
        public static readonly byte _maxDegreeOfParallelism = Convert.ToByte(Math.Ceiling(Environment.ProcessorCount * 0.75));

        #region DI 
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        private readonly ISecretsManager _secretsManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        #endregion

        #region Bot
        private bool mIsAutoBanBotsEnabled = true;
        internal BotConfig BotConfiguration { get; private set; }
        internal DiscordClient Client { get; private set; }
        internal CommandsNextExtension Commands { get; private set; }
        #endregion

        #region EventDelegates
        private readonly AsyncEventHandler<DiscordClient, GuildMemberUpdateEventArgs> _guildMemberUpdatedHandler;
        public readonly AsyncEventHandler<DiscordClient, GuildRoleCreateEventArgs> _guildRoleCreatedHandler;
        public readonly AsyncEventHandler<DiscordClient, GuildRoleDeleteEventArgs> _guildRoleDeletedHandler;
        public readonly AsyncEventHandler<DiscordClient, GuildRoleUpdateEventArgs> _guildRoleUpdatedHandler;
        #endregion

        public MandrilDiscordBot(ILoggerFactory aLoggerFactory, ISecretsManager aSecretsManager, IConfiguration aConfiguration, IServiceScopeFactory aServiceScopeFactory)
        {
            _loggerFactory = aLoggerFactory;
            _secretsManager = aSecretsManager;
            _configuration = aConfiguration;
            _serviceScopeFactory = aServiceScopeFactory;

            _guildMemberUpdatedHandler = (client, args) => GuildMemberUpdated?.Invoke(client, args);
            _guildRoleCreatedHandler = (client, args) => GuildRoleCreated?.Invoke(client, args);
            _guildRoleDeletedHandler = (client, args) => GuildRoleDeleted?.Invoke(client, args);
            _guildRoleUpdatedHandler = (client, args) => GuildRoleUpdated?.Invoke(client, args);

        }

        #region IMandrilDiscordBot

        public event AsyncEventHandler<DiscordClient, GuildMemberUpdateEventArgs> GuildMemberUpdated;
        public event AsyncEventHandler<DiscordClient, GuildRoleCreateEventArgs> GuildRoleCreated;
        public event AsyncEventHandler<DiscordClient, GuildRoleDeleteEventArgs> GuildRoleDeleted;
        public event AsyncEventHandler<DiscordClient, GuildRoleUpdateEventArgs> GuildRoleUpdated;

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
            AddEventHandlers(Client);
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
            Commands.RegisterCommands<BotTrustedMemberCommands>();
        }

        private IServiceProvider GetCommandsServiceProvider()
        {
            var lNewConfiguration = new ConfigurationBuilder()
                .AddConfiguration(_configuration)
                .Build();

            var lServices = new ServiceCollection();
            lServices.AddSingleton<IMandrilDiscordBot>(this);
            lServices.AddScoped<IMandrilMembersService, MandrilMembersService>();
            lServices.AddScoped<IMandrilRolesService, MandrilRolesService>();
            lServices.AddScoped<INewMemberManagementService, NewMemberManagementService>();
            lServices.AddSingleton<IConfiguration>(lNewConfiguration); // Register lNewConfiguration as IConfiguration
            return lServices.BuildServiceProvider();
        }

        #endregion

        #region BotEvents

        private void AddEventHandlers(DiscordClient aDiscordClient)
        {
            if (aDiscordClient == null)
                return;
            aDiscordClient.GuildMemberAdded += OnGuildMemberAdded;//By default the bot will be banning new bots unless it is temporary disabled via admin commands.
            aDiscordClient.GuildMemberUpdated += _guildMemberUpdatedHandler;
            aDiscordClient.GuildRoleCreated += _guildRoleCreatedHandler;
            aDiscordClient.GuildRoleDeleted += _guildRoleDeletedHandler;
            aDiscordClient.GuildRoleUpdated += _guildRoleUpdatedHandler;
        }

        private void RemoveEventHandlers(DiscordClient aDiscordClient)
        {
            if (aDiscordClient == null)
                return;
            aDiscordClient.GuildMemberAdded -= OnGuildMemberAdded;
            aDiscordClient.GuildMemberUpdated -= _guildMemberUpdatedHandler;
            aDiscordClient.GuildRoleCreated -= _guildRoleCreatedHandler;
            aDiscordClient.GuildRoleDeleted -= _guildRoleDeletedHandler;
            aDiscordClient.GuildRoleUpdated -= _guildRoleUpdatedHandler;
        }

        /// <summary>
        /// Ban every new bot that joins the server unless it is allowed for short time by an admin using the admin command "?open-bots".
        /// </summary>
        private async Task OnGuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            if (mIsAutoBanBotsEnabled && e.Guild.Id == BotConfiguration.DiscordTargetGuildId && e.Member.IsBot)
                await e.Member.BanAsync(reason: "Bot detected.");
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            RemoveEventHandlers(Client);
            Commands.Dispose();
            Client.Dispose();
        }
        #endregion

    }
}