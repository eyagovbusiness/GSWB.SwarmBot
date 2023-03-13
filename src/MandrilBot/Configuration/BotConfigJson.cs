using Newtonsoft.Json;

namespace MandrilBot.Configuration
{
    /// <summary>
    /// Secret deserialization type, Secret's keys have to match JssonPropery names
    /// </summary>
    public struct BotConfigJson
    {
        [JsonProperty("MandrilBotToken")]
        public string MandrilBotToken { get; set; }
        [JsonProperty("DiscordTargetGuildId")]
        public ulong DiscordTargetGuildId { get; set; }
        [JsonProperty("BotCommandPrefix")]
        public string BotCommandPrefix { get; set; }
    }
}
