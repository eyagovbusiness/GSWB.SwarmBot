using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Commands
{
    public class CreateRoleCommand : IRequest<IResult<string>>
    {
        public string RoleName { get; private set; }

        public CreateRoleCommand(string aRoleName)
        {
            RoleName = aRoleName;
        }

    }
}
