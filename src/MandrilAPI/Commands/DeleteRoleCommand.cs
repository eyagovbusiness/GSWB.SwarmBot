using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Commands
{
    public class DeleteRoleCommand : IRequest<IResult<Unit>>
    {
        public ulong RoleId { get; private set; }

        public DeleteRoleCommand(ulong aRoleId)
        {
            RoleId = aRoleId;
        }

    }
}
