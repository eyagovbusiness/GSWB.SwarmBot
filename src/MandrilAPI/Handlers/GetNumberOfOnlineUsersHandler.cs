using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class GetNumberOfOnlineUsersHandler : IRequestHandler<GetNumberOfOnlineUsersQuery, Result<int>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public GetNumberOfOnlineUsersHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result<int>> Handle(GetNumberOfOnlineUsersQuery aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.GetNumberOfOnlineUsers(aCancellationToken);
        }

    }
}

