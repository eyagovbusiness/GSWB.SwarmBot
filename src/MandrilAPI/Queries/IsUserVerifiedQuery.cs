using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Queries
{
    public class ExistDiscordUserQuery : IRequest<Result<bool>>
    {
        public ulong UserId { get; set; }

        public ExistDiscordUserQuery(ulong aUserId)
        {
            UserId = aUserId;
        }

    }
}
