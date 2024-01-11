using Mandril.Application.DTOs;
using TGF.CA.Infrastructure.Communication.Messages;

namespace Mandril.Infrastructure.Communication.Messages
{
    public record RoleCreated(DiscordRoleDTO DiscordRole);
}
