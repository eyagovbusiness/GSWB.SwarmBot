using MandrilAPI.Commands;
using MandrilBot;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class RevokeRoleToMemberListHandler : IRequestHandler<RevokeRoleToMemberListCommand, IResult<Unit>>
    {
        private readonly IRolesController _rolesController;
        public RevokeRoleToMemberListHandler(IRolesController aRolesController)
            => _rolesController = aRolesController;

        public async Task<IResult<Unit>> Handle(RevokeRoleToMemberListCommand aRequest, CancellationToken aCancellationToken)
            => await _rolesController.RevokeRoleToMemberList(aRequest.RoleId, aRequest.FullDiscordHandleList, aCancellationToken);

    }
}

