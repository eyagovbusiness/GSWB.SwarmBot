using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class AssignRoleToUserCommand : IRequest<Result>
    {
        public ulong RoleId { get; set; }
        public string FullDiscordIdentifier { get; set; }

        public AssignRoleToUserCommand(ulong aRoleId, string aFullDiscordIdentifier)
        {
            RoleId = aRoleId;
            FullDiscordIdentifier = aFullDiscordIdentifier;

        }

    }
}
