using MandrilAPI.Commands;
using MandrilBot;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, IResult<Unit>>
    {
        private readonly IRolesController _rolesController;
        public DeleteRoleHandler(IRolesController aRolesController)
        => _rolesController = aRolesController;

        public async Task<IResult<Unit>> Handle(DeleteRoleCommand aRequest, CancellationToken aCancellationToken)
            => await _rolesController.DeleteRole(aRequest.RoleId, aCancellationToken);

    }
}

