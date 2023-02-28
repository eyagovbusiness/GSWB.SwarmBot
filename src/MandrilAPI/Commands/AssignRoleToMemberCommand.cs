using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class AssignRoleToMemberCommand : IRequest<Result>
    {
        public ulong RoleId { get; set; }
        public string FullDiscordIdentifier { get; set; }

        public AssignRoleToMemberCommand(ulong aRoleId, string aFullDiscordIdentifier)
        {
            RoleId = aRoleId;
            FullDiscordIdentifier = aFullDiscordIdentifier;

        }

    }
}
