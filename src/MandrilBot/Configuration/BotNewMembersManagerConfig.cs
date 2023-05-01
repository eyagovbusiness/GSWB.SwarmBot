using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGF.Common.Serialization.Converters;

namespace MandrilBot.Configuration
{
    public class BotNewMembersManagerConfig
    {
        public BotNewMembersManagerConfig() { }

        [JsonConverter(typeof(UlongConverter))]
        public ulong NoMediaRoleId { get; set; }

        public byte BaseNoMediaDays { get; set; }

        public byte NoMediaDaysMultiplier { get; set; }

        [JsonConverter(typeof(UlongConverter))]
        public ulong MediaRoleId { get; set; }
    }
}
