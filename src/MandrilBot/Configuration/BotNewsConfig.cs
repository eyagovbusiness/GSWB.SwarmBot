
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
        public DevTracker DevTracker { get; set; }
    }

    /// <summary>
    /// Class used to deserialize needed part of appsettings.
    /// </summary>
    public class DevTracker
    {
        public DevTracker() { }
        /// <summary>
        /// Relative path of the resource for this <see cref="DevTracker"/>.
        /// </summary>
        public string ResourcePath { get; set; }
        /// <summary>
        /// Name of the <see cref="DiscordChannel"/> where the news will be sent.
        /// </summary>
        public string DiscordChannel { get; set; }
    }

}
