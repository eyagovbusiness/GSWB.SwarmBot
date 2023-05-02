using MandrilAPI.Commands;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class RevokeRoleToUserHandler : IRequestHandler<RevokeRoleToMemberCommand, IResult<Unit>>
    {
        private readonly IRolesController _rolesController;
        public RevokeRoleToUserHandler(IRolesController aRolesController)
            => _rolesController = aRolesController;

        public async Task<IResult<Unit>> Handle(RevokeRoleToMemberCommand aRequest, CancellationToken aCancellationToken)
            => await _rolesController.RevokeRoleToMemberList(aRequest.RoleId, new string[] { aRequest.FullDiscordIdentifier }, aCancellationToken);

    }
}

