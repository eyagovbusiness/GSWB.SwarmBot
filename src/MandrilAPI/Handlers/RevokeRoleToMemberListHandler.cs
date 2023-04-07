using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class RevokeRoleToMemberListHandler : IRequestHandler<RevokeRoleToMemberListCommand, IResult<Unit>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public RevokeRoleToMemberListHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<Unit>> Handle(RevokeRoleToMemberListCommand aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.RevokeRoleToMemberList(aRequest.RoleId, aRequest.FullDiscordHandleList, aCancellationToken);

    }
}

