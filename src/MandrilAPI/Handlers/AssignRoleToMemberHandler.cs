using MandrilAPI.Commands;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class AssignRoleToMemberHandler : IRequestHandler<AssignRoleToMemberCommand, IResult<Unit>>
    {
        private readonly IRolesController _rolesController;
        public AssignRoleToMemberHandler(IRolesController aRolesController)
            => _rolesController = aRolesController;

        public async Task<IResult<Unit>> Handle(AssignRoleToMemberCommand aRequest, CancellationToken aCancellationToken)
            => await _rolesController.AssignRoleToMember(aRequest.RoleId, aRequest.FullDiscordIdentifier, aCancellationToken: aCancellationToken);

    }
}

