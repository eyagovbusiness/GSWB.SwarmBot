using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class RevokeRoleToUserHandler : IRequestHandler<RevokeRoleToMemberCommand, Result>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public RevokeRoleToUserHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result> Handle(RevokeRoleToMemberCommand aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.RevokeRoleToMemberList(aRequest.RoleId, new string[] { aRequest.FullDiscordIdentifier }, aCancellationToken);
        }

    }
}

