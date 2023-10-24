
namespace Mandril.Infrastructure.Communication.Messages
{
    public record MemberAvatarUpdateDTO(ulong DiscordUserId, string NewAvatarUrl);
}
