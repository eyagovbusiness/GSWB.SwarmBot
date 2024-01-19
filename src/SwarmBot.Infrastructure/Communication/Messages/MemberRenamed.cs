
using TGF.CA.Infrastructure.Communication.Messages;

namespace SwarmBot.Infrastructure.Communication.Messages
{
    public record MemberRenamed(string DiscordUserId, string NewDisplayName);
}
