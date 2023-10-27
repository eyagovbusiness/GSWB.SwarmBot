
namespace Mandril.Application.DTOs.Messages
{
    public record MemberRoleRevokedDTO(string DiscordUserId, DiscordRoleDTO[] DiscordRoleList);
}
