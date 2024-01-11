
using TGF.CA.Infrastructure.Communication.Messages;

namespace Mandril.Infrastructure.Communication.Messages
{
    public record MemberAvatarUpdated(string DiscordUserId, string NewAvatarUrl);
}
