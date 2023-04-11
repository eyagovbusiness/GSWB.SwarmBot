using MandrilAPI.Commands;
using MandrilBot;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class AssignRoleToMemberListHandler : IRequestHandler<AssignRoleToMemberListCommand, IResult<Unit>>
    {
        private readonly IRolesController _rolesController;
        public AssignRoleToMemberListHandler(IRolesController aRolesController)
            => _rolesController = aRolesController;

        public async Task<IResult<Unit>> Handle(AssignRoleToMemberListCommand aRequest, CancellationToken aCancellationToken)
            => await _rolesController.AssignRoleToMemberList(aRequest.RoleId, aRequest.FullDiscordIdentifierList, aCancellationToken);

    }
}

