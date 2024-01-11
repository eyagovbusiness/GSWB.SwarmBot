using Mandril.Application.DTOs;
using TGF.CA.Infrastructure.Communication.Messages;

namespace Mandril.Infrastructure.Communication.Messages
{
    public record MemberRoleRevoked(string DiscordUserId, DiscordRoleDTO[] DiscordRoleList);
}
