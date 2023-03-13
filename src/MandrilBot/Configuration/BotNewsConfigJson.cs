
namespace MandrilBot.Configuration
{
    /// <summary>
    /// Class used to deserialize needed part of appsettings.
    /// </summary>
    public class BotNewsConfig
    {
        public BotNewsConfig() { }
        public string BaseResourceAddress { get; set; }
        public string CitizensPath { get; set; }
        public DevTracker DevTracker { get; set; }
    }

    /// <summary>
    /// Class used to deserialize needed part of appsettings.
    /// </summary>
    public class DevTracker
    {
        public DevTracker() { }
        public string ResourcePath { get; set; }
        public string DiscordChannel { get; set; }
    }

}
