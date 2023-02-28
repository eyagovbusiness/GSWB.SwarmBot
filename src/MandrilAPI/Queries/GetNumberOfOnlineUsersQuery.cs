using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Queries
{
    public class GetNumberOfOnlineUsersQuery : IRequest<Result<int>>
    {
        public GetNumberOfOnlineUsersQuery()
        {
        }

    }
}
