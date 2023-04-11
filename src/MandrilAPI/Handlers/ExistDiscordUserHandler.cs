using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class ExistDiscordUserHandler : IRequestHandler<ExistDiscordUserQuery, IResult<bool>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public ExistDiscordUserHandler(IMandrilDiscordBot aMandrilDiscordBot)
         => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<bool>> Handle(ExistDiscordUserQuery aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.ExistUser(aRequest.UserId, aCancellationToken);

    }
}

