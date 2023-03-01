using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MandrilBot
{
    /// <summary>
    /// Class designed as a service to provide comminication with an internal Discord bot in this service, with functionality accessible through the public interface.
    /// </summary>
    public partial class MandrilDiscordBot : IMandrilDiscordBot
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        internal BotConfigJson _botConfiguration;

        internal DiscordClient Client { get; private set; }
        internal CommandsNextExtension Commands { get; private set; }

        public MandrilDiscordBot(IConfiguration aConfiguration, ILoggerFactory aLoggerFactory)
        {
            _logger = aLoggerFactory.CreateLogger(typeof(MandrilDiscordBot));
            _botConfiguration = aConfiguration.Get<BotConfigJson>();
            _loggerFactory = aLoggerFactory;
        }

        /// <summary>
        /// Attempts asynchronously to establish the connection of the internal configured bot with Discord.
        /// </summary>
        /// <returns>awaitable <see cref="Task"/></returns>
        public async Task StartAsync()
        {
            try
            {
                var config = new DiscordConfiguration
                {
                    LoggerFactory = _loggerFactory,
                    MinimumLogLevel = LogLevel.Error,
                    Token = _botConfiguration.MandrilBotToken,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    Intents = DiscordIntents.MessageContents
                              | DiscordIntents.GuildMessages
                              | DiscordIntents.Guilds
                              | DiscordIntents.GuildVoiceStates
                              | DiscordIntents.GuildPresences
                };

                Client = new DiscordClient(config);

                //Client.Ready += Client_Ready;
                //Client.VoiceStateUpdated += Client_VoiceStateUpdated;
                //Client.PresenceUpdated += Client_PresenceUpdated;

                var lCommandsConfig = new CommandsNextConfiguration
                {
                    StringPrefixes = new string[] { _botConfiguration.BotCommandPrefix },
                    EnableDms = false,
                    EnableMentionPrefix = true,
                };

                Commands = Client.UseCommandsNext(lCommandsConfig);
                Commands.RegisterCommands<BotCommands>();

                await Client.ConnectAsync();

            }
            catch (Exception lException)
            {
                _logger.LogError("An error occurred while attempting to start the Discord bot client: ", lException.ToString());
            }
        }

        //private Task Client_PresenceUpdated(DiscordClient sender, DSharpPlus.EventArgs.PresenceUpdateEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //private Task Client_VoiceStateUpdated(DiscordClient sender, DSharpPlus.EventArgs.VoiceStateUpdateEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        //private Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        //{
        //    throw new NotImplementedException()
        //}
    }
}