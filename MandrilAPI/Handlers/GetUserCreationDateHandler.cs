using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class GetUserCreationDateHandler : IRequestHandler<GetUserCreationDateQuery, Result<DateTimeOffset>>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public GetUserCreationDateHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result<DateTimeOffset>> Handle(GetUserCreationDateQuery aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.GetUserCreationDate(aRequest.UserId, aCancellationToken);
        }

    }
}

