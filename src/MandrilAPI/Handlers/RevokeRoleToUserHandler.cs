using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class RevokeRoleToUserHandler : IRequestHandler<RevokeRoleToMemberCommand, IResult<Unit>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public RevokeRoleToUserHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<Unit>> Handle(RevokeRoleToMemberCommand aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.RevokeRoleToMemberList(aRequest.RoleId, new string[] { aRequest.FullDiscordIdentifier }, aCancellationToken);

    }
}

