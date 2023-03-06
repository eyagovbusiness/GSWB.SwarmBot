using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class AssignRoleToMemberListCommand : IRequest<Result>
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
