using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class CreateRoleCommand : IRequest<Result<string>>
    {
        public string RoleName { get; private set; }

        public CreateRoleCommand(string aRoleName)
        {
            RoleName = aRoleName;
        }

    }
}
