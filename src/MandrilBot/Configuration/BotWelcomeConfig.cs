using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MandrilBot.Configuration
{
    public class BotWelcomeConfig
    {
        public BotWelcomeConfig() { }

        [JsonConverter(typeof(UlongConverter))]
        public ulong NoMediaRoleId { get; set; }

        public byte BaseNoMediaDays { get; set; }

        public byte NoMediaDaysMultiplier { get; set; }

        [JsonConverter(typeof(UlongConverter))]
        public ulong MediaRoleId { get; set; }
    }
    public class UlongConverter : Newtonsoft.Json.JsonConverter<ulong>
    {
        public override ulong ReadJson(JsonReader reader, Type objectType, ulong existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string stringValue = (string)reader.Value;
                if (ulong.TryParse(stringValue, out ulong result))
                {
                    return result;
                }
            }

            throw new JsonSerializationException($"Failed to deserialize {objectType.Name} from JSON.");
        }

        public override void WriteJson(JsonWriter writer, ulong value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
