using DSharpPlus.Entities;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Queries
{
    public class GetMemberHighestRoleQuery : IRequest<IResult<DiscordRole>>
    {
        public ulong UserId { get; private set; }

        public GetMemberHighestRoleQuery(ulong aUserId)
        {
            UserId = aUserId;
        }

    }
}
