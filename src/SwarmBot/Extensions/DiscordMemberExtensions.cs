using DSharpPlus;
using DSharpPlus.Entities;

namespace SwarmBot.Extensions
{
    public static class DiscordMemberExtensions
    {
        public const int DefaultAvatarSize = 64;
        public const ImageFormat DefaultAvatarFormat = ImageFormat.Jpeg;
        public static string GetGuildAvatarUrlOrDefault(this DiscordMember aDiscordMember)
        => aDiscordMember.GetGuildAvatarUrl(DefaultAvatarFormat, DefaultAvatarSize)
        ?? aDiscordMember.DefaultAvatarUrl;

    }
}
