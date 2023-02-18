using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class AssignRoleToUserListCommand : IRequest<Result>
    {
        public ulong RoleId { get; set; }
        public string[] FullDiscordIdentifierList { get; set; }

        public AssignRoleToUserListCommand(ulong aRoleId, string[] aFullDiscordIdentifierList)
        {
            RoleId = aRoleId;
            FullDiscordIdentifierList = aFullDiscordIdentifierList;

        }

    }
}
