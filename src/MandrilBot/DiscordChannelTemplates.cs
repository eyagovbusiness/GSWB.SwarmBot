using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MandrilBot
{
    [JsonObject]
    public struct CategoryChannelTemplate
    {
        [JsonPropertyName("ChannelType")]
        internal readonly DSharpPlus.ChannelType ChannelType => DSharpPlus.ChannelType.Category;

        [JsonPropertyName("Name")]
        public string Name;

        [JsonPropertyName("ChannelList")]
        public ChannelTemplate[] ChannelList;

        [JsonPropertyName("Position")]
        public int? Position;
    }
    [JsonObject]
    public struct ChannelTemplate
    {
        [JsonPropertyName("ChannelType")]
        public DSharpPlus.ChannelType ChannelType;

        [JsonPropertyName("Name")]
        public string Name;

        [JsonPropertyName("Position")]
        public int? Position;
    }
}
