using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class IsUserVerifiedHandler : IRequestHandler<IsUserVerifiedQuery, IResult<bool>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public IsUserVerifiedHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<bool>> Handle(IsUserVerifiedQuery aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.IsUserVerified(aRequest.UserId, aCancellationToken);

    }
}

