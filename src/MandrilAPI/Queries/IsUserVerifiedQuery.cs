using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Queries
{
    public class ExistDiscordUserQuery : IRequest<Result<bool>>
    {
        public ulong UserId { get; private set; }

        public ExistDiscordUserQuery(ulong aUserId)
        {
            UserId = aUserId;
        }

    }
}
