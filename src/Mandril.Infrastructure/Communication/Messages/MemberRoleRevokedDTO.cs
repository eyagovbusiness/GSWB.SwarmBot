using Mandril.Application.DTOs;

namespace Mandril.Infrastructure.Communication.Messages
{
    public record MemberRoleRevokedDTO(string DiscordUserId, DiscordRoleDTO[] DiscordRoleList);
}
