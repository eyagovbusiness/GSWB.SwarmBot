using Mandril.Application.DTOs;

namespace Mandril.Infrastructure.Communication.Messages
{
    public record MemberRoleRevokedDTO(ulong DiscordUserId, DiscordRoleDTO[] DiscordRoleList);
}
