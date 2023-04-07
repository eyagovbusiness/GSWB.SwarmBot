using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Commands
{
    public class AssignRoleToMemberCommand : IRequest<IResult<Unit>>
    {
        public ulong RoleId { get; private set; }
        public string FullDiscordIdentifier { get; private set; }

        public AssignRoleToMemberCommand(ulong aRoleId, string aFullDiscordIdentifier)
        {
            RoleId = aRoleId;
            FullDiscordIdentifier = aFullDiscordIdentifier;

        }

    }
}
