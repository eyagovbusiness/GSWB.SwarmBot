using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Queries
{
    public class GetUserCreationDateQuery : IRequest<IResult<DateTimeOffset>>
    {
        public ulong UserId { get; private set; }

        public GetUserCreationDateQuery(ulong aUserId)
        {
            UserId = aUserId;
        }

    }
}
