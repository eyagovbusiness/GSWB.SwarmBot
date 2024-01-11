
using TGF.CA.Infrastructure.Communication.Messages;

namespace Mandril.Infrastructure.Communication.Messages
{
    public record MemberRenamed(string DiscordUserId, string NewDisplayName);
}
