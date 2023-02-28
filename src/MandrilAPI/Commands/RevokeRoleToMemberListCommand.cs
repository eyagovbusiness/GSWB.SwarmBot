using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class RevokeRoleToMemberListCommand : IRequest<Result>
    {
        public ulong RoleId { get; set; }
        public string[] FullDiscordHandleList { get; set; }

        public RevokeRoleToMemberListCommand(ulong aRoleId, string[] aFullDiscordHandleList)
        {
            RoleId = aRoleId;
            FullDiscordHandleList = aFullDiscordHandleList;

        }

    }
}
