using SwarmBot.Application.DTOs;
using TGF.CA.Infrastructure.Communication.Messages;

namespace SwarmBot.Infrastructure.Communication.Messages
{
    public record RoleUpdated(DiscordRoleDTO DiscordRole);
}
