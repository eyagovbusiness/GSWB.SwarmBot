using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Queries
{
    public class GetNumberOfOnlineUsersQuery : IRequest<Result<int>>
    {
        public GetNumberOfOnlineUsersQuery()
        {
        }

    }
}
