using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class DeleteRoleCommand : IRequest<Result>
    {
        public ulong RoleId { get; private set; }

        public DeleteRoleCommand(ulong aRoleId)
        {
            RoleId = aRoleId;
        }

    }
}
