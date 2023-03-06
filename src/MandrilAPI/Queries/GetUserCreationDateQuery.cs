using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Queries
{
    public class GetUserCreationDateQuery : IRequest<Result<DateTimeOffset>>
    {
        public ulong UserId { get; private set; }

        public GetUserCreationDateQuery(ulong aUserId)
        {
            UserId = aUserId;
        }

    }
}
