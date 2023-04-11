using MediatR;
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
