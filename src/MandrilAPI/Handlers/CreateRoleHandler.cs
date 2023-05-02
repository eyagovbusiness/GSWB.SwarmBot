using MandrilAPI.Commands;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, IResult<string>>
    {
        private readonly IRolesController _rolesController;
        public CreateRoleHandler(IRolesController aRolesController)
            => _rolesController = aRolesController;

        public async Task<IResult<string>> Handle(CreateRoleCommand aRequest, CancellationToken aCancellationToken)
            => await _rolesController.CreateRole(aRequest.RoleName, aCancellationToken);

    }
}

