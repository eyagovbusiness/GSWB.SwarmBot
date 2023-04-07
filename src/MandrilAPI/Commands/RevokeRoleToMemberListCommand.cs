using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Commands
{
    public class RevokeRoleToMemberListCommand : IRequest<IResult<Unit>>
    {
        public ulong RoleId { get; private set; }
        public string[] FullDiscordHandleList { get; private set; }

        public RevokeRoleToMemberListCommand(ulong aRoleId, string[] aFullDiscordHandleList)
        {
            RoleId = aRoleId;
            FullDiscordHandleList = aFullDiscordHandleList;

        }

    }
}
