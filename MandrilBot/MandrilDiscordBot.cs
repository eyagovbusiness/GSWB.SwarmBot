using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace MandrilBot
{
    public class MandrilDiscordBot
    {
        internal BotConfigJson _config;
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public async Task RunAsync()
        {
            _config = await GetBotConfig();

            var config = new DiscordConfiguration
            {
                Token = _config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.All
            };

            Client = new DiscordClient(config);

            Client.Ready += Client_Ready;

            var lCommandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { _config.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
            };

            Commands = Client.UseCommandsNext(lCommandsConfig);

            Commands.RegisterCommands<BotCommands>();

            await Client.ConnectAsync();

            await Task.Delay(-1);

        }

        private Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            return null;
        }

        private async Task<BotConfigJson> GetBotConfig()
        {
            var lJson = string.Empty;
            using (var lFileStream = File.OpenRead("..\\MandrilBot\\BotConfig.json"))
            {
                using (var lStreamReader = new StreamReader(lFileStream, new UTF8Encoding(false)))
                {
                    lJson = await lStreamReader.ReadToEndAsync().ConfigureAwait(false);
                }
            }
            var lConfigJson = JsonConvert.DeserializeObject<BotConfigJson>(lJson);
            return lConfigJson;
        }

    }
}