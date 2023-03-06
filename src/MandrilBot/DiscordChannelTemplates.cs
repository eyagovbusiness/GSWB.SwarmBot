using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MandrilBot
{
    [JsonObject]
    public class CategoryChannelTemplate
    {
        public CategoryChannelTemplate(){ }

        [JsonPropertyName("ChannelType")]
        public DSharpPlus.ChannelType ChannelType { get; private set; } = DSharpPlus.ChannelType.Category;

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("ChannelList")]
        public ChannelTemplate[] ChannelList { get; set; }

        [JsonPropertyName("Position")]
        public int? Position { get; set; }
    }
    [JsonObject]
    public class ChannelTemplate
    {
        public ChannelTemplate() { }

        [JsonPropertyName("ChannelType")]
        public DSharpPlus.ChannelType ChannelType { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Position")]
        public int? Position { get; set; }
    }
}
