using Newtonsoft.Json;

namespace MandrilBot.Configuration
{
    /// <summary>
    /// Secret deserialization type, Secret's keys have to match JssonPropery names
    /// </summary>
    //public struct BotNewsConfigJson
    //{
    //    [JsonProperty("MandrilBotToken")]
    //    public string MandrilBotToken { get; set; }
    //    [JsonProperty("DiscordTargetGuildId")]
    //    public ulong DiscordTargetGuildId { get; set; }
    //    [JsonProperty("BotNews")]
    //    public BotNews BotNews { get; set; }
    //}
    public class BotNewsConfig
    {
        public BotNewsConfig() { }
        public string BaseResourceAddress { get; set; }
        public string CitizensPath { get; set; }
        public DevTracker DevTracker { get; set; }
    }
    public class DevTracker
    {
        public DevTracker() { }
        public string ResourcePath { get; set; }
        public string DiscordChannel { get; set; }
    }

}
