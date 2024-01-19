
using TGF.CA.Infrastructure.Communication.Messages;

namespace SwarmBot.Infrastructure.Communication.Messages
{
    public record MemberAvatarUpdated(string DiscordUserId, string NewAvatarUrl);
}
