using MandrilAPI.Queries;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class GetNumberOfOnlineMembersHandler : IRequestHandler<GetNumberOfOnlineMembersQuery, IResult<int>>
    {
        private readonly IGuildController _guildController;
        public GetNumberOfOnlineMembersHandler(IGuildController aGuildController)
            => _guildController = aGuildController;

        public async Task<IResult<int>> Handle(GetNumberOfOnlineMembersQuery aRequest, CancellationToken aCancellationToken)
            => await _guildController.GetNumberOfOnlineMembers(aCancellationToken);

    }
}

