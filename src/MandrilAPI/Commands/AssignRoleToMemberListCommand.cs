using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Commands
{
    public class AssignRoleToMemberListCommand : IRequest<IResult<Unit>>
    {
        public ulong RoleId { get; private set; }
        public string[] FullDiscordIdentifierList { get; private set; }

        public AssignRoleToMemberListCommand(ulong aRoleId, string[] aFullDiscordIdentifierList)
        {
            RoleId = aRoleId;
            FullDiscordIdentifierList = aFullDiscordIdentifierList;

        }

    }
}
