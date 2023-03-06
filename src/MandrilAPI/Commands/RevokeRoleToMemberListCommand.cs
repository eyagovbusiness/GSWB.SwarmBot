using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class RevokeRoleToMemberListCommand : IRequest<Result>
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
