using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class RevokeRoleToMemberListHandler : IRequestHandler<RevokeRoleToMemberListCommand, Result>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public RevokeRoleToMemberListHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result> Handle(RevokeRoleToMemberListCommand aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.RevokeRoleToMemberList(aRequest.RoleId, aRequest.FullDiscordHandleList, aCancellationToken);
        }

    }
}

