using DSharpPlus.Entities;
using Mandril.Application.DTOs;

namespace Mandril.Application.Mapping
{
    public static class DiscordRoleMapping
    {
        public static DiscordRoleDTO ToDto(this DiscordRole aDiscordRole)
            => new(aDiscordRole.Id.ToString(), aDiscordRole.Name, (byte)aDiscordRole.Position);
    }
}
