using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class GetNumberOfOnlineMembersHandler : IRequestHandler<GetNumberOfOnlineMembersQuery, IResult<int>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public GetNumberOfOnlineMembersHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<int>> Handle(GetNumberOfOnlineMembersQuery aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.GetNumberOfOnlineMembers(aCancellationToken);

    }
}

