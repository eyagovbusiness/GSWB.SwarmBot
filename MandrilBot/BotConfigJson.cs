using Newtonsoft.Json;

namespace MandrilBot
{

    public struct BotConfigJson
    {
        [JsonProperty("Token")]
        public string Token { get; set; }
        [JsonProperty("GuildId")]
        public ulong GuildId { get; set; }
        [JsonProperty("Prefix")]
        public string Prefix { get; set; }
    }
}
