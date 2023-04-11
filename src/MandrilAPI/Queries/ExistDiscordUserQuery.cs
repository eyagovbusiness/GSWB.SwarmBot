using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Queries
{
    public class IsUserVerifiedQuery : IRequest<IResult<bool>>
    {
        public ulong UserId { get; private set; }

        public IsUserVerifiedQuery(ulong aUserId)
        {
            UserId = aUserId;
        }

    }
}
