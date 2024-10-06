using Common.Application.DTOs.Guilds;
using DSharpPlus.Entities;

namespace SwarmBot.Mappings
{
    public static class DiscordGuildMappings
    {
        public static GuildDTO ToDto(this DiscordGuild discordGuild)
            => new(discordGuild.Id.ToString(), discordGuild.Name, discordGuild.IconUrl);
    }
}
