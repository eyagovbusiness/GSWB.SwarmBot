using Mandril.Application.DTOs;

namespace Mandril.Infrastructure.Communication.Messages
{
    public record MemberRoleAddedDTO(ulong DiscordUserId, DiscordRoleDTO[] DiscordRoleList);
}
