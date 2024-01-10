
namespace Mandril.Application.DTOs.Messages
{
    public record MemberRoleAssignedDTO(string DiscordUserId, DiscordRoleDTO[] DiscordRoleList);
}
