using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class IsUserVerifiedHandler : IRequestHandler<IsUserVerifiedQuery, Result<bool>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public IsUserVerifiedHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result<bool>> Handle(IsUserVerifiedQuery aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.IsUserVerified(aRequest.UserId, aCancellationToken);
        }

    }
}

