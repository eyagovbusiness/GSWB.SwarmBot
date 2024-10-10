namespace SwarmBot
{
    /// <summary>
    /// Secret deserialization type, Secret's keys have to match JsonPropery names
    /// </summary>
    public class BotConfig
    {
        public string? BotToken { get; set; }
        public ulong DiscordTargetGuildId { get; set; }
        public ulong TestersDiscordTargetGuildId { get; set; }
        public string? BotCommandPrefix { get; set; }
    }
}
