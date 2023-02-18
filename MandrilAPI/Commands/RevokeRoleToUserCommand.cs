using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class RevokeRoleToUserCommand : IRequest<Result>
    {
        public ulong RoleId { get; set; }
        public string FullDiscordIdentifier { get; set; }

        public RevokeRoleToUserCommand(ulong aRoleId, string aFullDiscordIdentifier)
        {
            RoleId = aRoleId;
            FullDiscordIdentifier = aFullDiscordIdentifier;

        }

    }
}
