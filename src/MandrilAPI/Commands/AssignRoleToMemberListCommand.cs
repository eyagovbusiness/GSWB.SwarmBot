using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class AssignRoleToMemberListCommand : IRequest<Result>
    {
        public ulong RoleId { get; set; }
        public string[] FullDiscordIdentifierList { get; set; }

        public AssignRoleToMemberListCommand(ulong aRoleId, string[] aFullDiscordIdentifierList)
        {
            RoleId = aRoleId;
            FullDiscordIdentifierList = aFullDiscordIdentifierList;

        }

    }
}
