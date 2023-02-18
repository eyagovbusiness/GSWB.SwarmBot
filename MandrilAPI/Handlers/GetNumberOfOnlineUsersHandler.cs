using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class GetNumberOfOnlineUsersHandler : IRequestHandler<GetNumberOfOnlineUsersQuery, Result<int>>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public GetNumberOfOnlineUsersHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public Task<Result<int>> Handle(GetNumberOfOnlineUsersQuery aRequest, CancellationToken aCancellationToken)
        {
            return _mandtrilDiscordBot.GetNumberOfOnlineUsers();
        }

    }
}

