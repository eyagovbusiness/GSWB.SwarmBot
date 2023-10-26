using Mandril.Application.DTOs;

namespace Mandril.Infrastructure.Communication.Messages
{
    public record MemberRoleAddedDTO(string DiscordUserId, DiscordRoleDTO[] DiscordRoleList);
}
