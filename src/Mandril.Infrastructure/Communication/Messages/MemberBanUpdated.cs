
using TGF.CA.Infrastructure.Communication.Messages;

namespace Mandril.Infrastructure.Communication.Messages
{
    /// <summary>
    /// Message sent when a guild ban event is rised, this DTO contains information about the target member id and the guild id where the event happened. Finally it contains formation about the member ban status after the update(<see cref="IsMemberBanned"/> determining if the event was a ban or an unban event).
    /// </summary>
    /// <param name="IsMemberBanned">If the member is banned or not after the update(this determines if the event was a ban or unban event).</param>
    public record MemberBanUpdated(string DiscordUserId, string DiscordGuildId, bool IsMemberBanned);
}
