using SwarmBot.Application.DTOs;
using TGF.CA.Infrastructure.Communication.Messages;

namespace SwarmBot.Infrastructure.Communication.Messages
{
    public record RoleCreated(DiscordRoleDTO DiscordRole);
}
