using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class CreateRoleCommand : IRequest<Result<string>>
    {
        public string RoleName { get; set; }

        public CreateRoleCommand(string aRoleName)
        {
            RoleName = aRoleName;
        }

    }
}
