using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Queries
{
    public class ExistDiscordUserQuery : IRequest<IResult<bool>>
    {
        public ulong UserId { get; private set; }

        public ExistDiscordUserQuery(ulong aUserId)
        {
            UserId = aUserId;
        }

    }
}
