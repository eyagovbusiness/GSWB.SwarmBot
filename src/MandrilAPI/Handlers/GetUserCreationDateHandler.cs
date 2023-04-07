using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class GetUserCreationDateHandler : IRequestHandler<GetUserCreationDateQuery, IResult<DateTimeOffset>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public GetUserCreationDateHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<DateTimeOffset>> Handle(GetUserCreationDateQuery aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.GetUserCreationDate(aRequest.UserId, aCancellationToken);

    }
}

