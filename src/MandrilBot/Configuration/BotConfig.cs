using Newtonsoft.Json;

namespace MandrilBot.Configuration
{
    /// <summary>
    /// Secret deserialization type, Secret's keys have to match JsonPropery names
    /// </summary>
    public class BotConfig
    {
        public string MandrilBotToken { get; set; }
        public ulong DiscordTargetGuildId { get; set; }
        public string BotCommandPrefix { get; set; }
    }
}
