using DSharpPlus.Entities;
using MandrilAPI.Queries;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class GetMemberHighestRoleHandler : IRequestHandler<GetMemberHighestRoleQuery, IResult<DiscordRole>>
    {
        private readonly IMembersController _membersController;
        public GetMemberHighestRoleHandler(IMembersController aMembersController)
            => _membersController = aMembersController;

        public async Task<IResult<DiscordRole>> Handle(GetMemberHighestRoleQuery aRequest, CancellationToken aCancellationToken)
            => await _membersController.GetMemberHighestRole(aRequest.UserId, aCancellationToken);

    }
}

