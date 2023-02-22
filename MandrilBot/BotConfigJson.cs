using Newtonsoft.Json;

namespace MandrilBot
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
        [JsonProperty("Prefix")]
        public string Prefix { get; set; }
    }
}
