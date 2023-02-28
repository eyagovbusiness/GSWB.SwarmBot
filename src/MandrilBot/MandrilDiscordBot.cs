using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MandrilBot
{
    public partial class MandrilDiscordBot : IMandrilDiscordBot
    {
        //private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        internal BotConfigJson _botConfiguration;

        internal DiscordClient Client { get; private set; }
        internal CommandsNextExtension Commands { get; private set; }

        public MandrilDiscordBot(/*IServiceProvider aServiceProvider,*/ IConfiguration aConfiguration, ILoggerFactory aLoggerFactory)
        {
            //_serviceProvider = aServiceProvider;
            _logger = aLoggerFactory.CreateLogger(typeof(MandrilDiscordBot));
            _botConfiguration = aConfiguration.Get<BotConfigJson>();
            _loggerFactory = aLoggerFactory;
        }

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
                _logger.LogError(lException.ToString());
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

        //private async Task<BotConfigJson> UpdateBotConfig()
        //{
        //    var lJson = string.Empty;
        //    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        //    string lBotConfigPath = string.Empty;
        //    if (env == "Production")
        //        lBotConfigPath = "BotConfig.json";
        //    if (env == "Development")
        //        lBotConfigPath = "../src/MandrilBot/BotConfig.json";

        //    using (var lFileStream = File.OpenRead(lBotConfigPath))
        //    {
        //        using (var lStreamReader = new StreamReader(lFileStream, new UTF8Encoding(false)))
        //        {
        //            lJson = await lStreamReader.ReadToEndAsync().ConfigureAwait(false);
        //        }
        //    }
        //    var lConfigJson = JsonConvert.DeserializeObject<BotConfigJson>(lJson);
        //    return lConfigJson;
        //} 

    }
}