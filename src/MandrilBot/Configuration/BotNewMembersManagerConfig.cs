using Newtonsoft.Json;
using TGF.Common.Serialization.Converters;

namespace MandrilBot.Configuration
{
    public class BotNewMembersManagerConfig
    {
        public BotNewMembersManagerConfig() { }

        [JsonConverter(typeof(UlongConverter))]
        public ulong NoMediaRoleId { get; set; }

        public byte NoMediaDays { get; set; }

        [JsonConverter(typeof(UlongConverter))]
        public ulong MediaRoleId { get; set; }
    }
}
