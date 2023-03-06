using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Queries
{
    public class IsUserVerifiedQuery : IRequest<Result<bool>>
    {
        public ulong UserId { get; private set; }

        public IsUserVerifiedQuery(ulong aUserId)
        {
            UserId = aUserId;
        }

    }
}
