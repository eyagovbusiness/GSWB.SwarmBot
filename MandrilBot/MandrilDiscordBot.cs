using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;

namespace MandrilBot
{
    public class MandrilDiscordBot
    {
        internal BotConfigJson _config;
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        private MandrilDiscordBot(BotConfigJson aBotConfig)
        {
            _config = aBotConfig;

        }
        public static async Task<MandrilDiscordBot> CreateAsync(BotConfigJson aBotConfig)
        {
            MandrilDiscordBot lNewDiscordBot = new MandrilDiscordBot(aBotConfig);
            await lNewDiscordBot.RunAsync();
            return lNewDiscordBot;
        }

        public async Task RunAsync()
        {
            try
            {
                var config = new DiscordConfiguration
                {

                    Token = _config.MandrilBotToken,
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    MinimumLogLevel = LogLevel.Error,
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
                    StringPrefixes = new string[] { _config.Prefix },
                    EnableDms = false,
                    EnableMentionPrefix = true,
                };

                Commands = Client.UseCommandsNext(lCommandsConfig);

                Commands.RegisterCommands<BotCommands>();

                await Client.ConnectAsync();

            }
            catch
            {
                //Log the exception
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
        //    throw new NotImplementedException();
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