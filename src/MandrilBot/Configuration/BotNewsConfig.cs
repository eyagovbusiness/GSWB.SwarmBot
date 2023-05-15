using Newtonsoft.Json;
using TGF.Common.Serialization.Converters;

namespace MandrilBot.Configuration
{
    /// <summary>
    /// Class used to deserialize needed part of appsettings.
    /// </summary>
    public class BotNewsConfig
    {
        public BotNewsConfig() { }
        /// <summary>
        /// Base address of all the resources with relative path in this configuration.
        /// </summary>
        public string BaseResourceAddress { get; set; }
        /// <summary>
        /// Relative path of the resource for all accounts in StarCitizen(including dev accounts).
        /// </summary>
        public string CitizensPath { get; set; }
        /// <summary>
        /// Resource of the Spectrum Logo.
        /// </summary>
        public string SpectrumLogoUri { get; set; }
        /// <summary>
        /// Configuration of the DevTracker settings section
        /// </summary>
        public NewsTopicConfig DevTracker { get; set; }

        public NewsTopicConfig CommLink { get; set; }
    }

    /// <summary>
    /// Class used to deserialize needed part of appsettings.
    /// </summary>
    public class NewsTopicConfig
    {
        public NewsTopicConfig() { }
        /// <summary>
        /// Relative path of the resource for this <see cref="NewsTopicConfig"/>.
        /// </summary>
        public string ResourcePath { get; set; }
        /// <summary>
        /// Name of the <see cref="DiscordChannelId"/> where the news will be sent.
        /// </summary>
        [JsonConverter(typeof(UlongConverter))]
        public ulong DiscordChannelId { get; set; }
    }


}
