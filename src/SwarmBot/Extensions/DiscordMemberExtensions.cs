using DSharpPlus;
using DSharpPlus.Entities;

namespace SwarmBot.Extensions
{
    public static class DiscordMemberExtensions
    {
        public const ushort DefaultAvatarSize = 256;
        public const ImageFormat DefaultAvatarFormat = ImageFormat.Png;
        public static string GetGuildAvatarUrlOrDefault(this DiscordMember aDiscordMember)
        => aDiscordMember.GetGuildAvatarUrl(DefaultAvatarFormat, DefaultAvatarSize)
        ?? aDiscordMember.DefaultAvatarUrl;

    }
}
