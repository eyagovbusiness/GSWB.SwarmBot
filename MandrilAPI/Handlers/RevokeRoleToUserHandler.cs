using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class RevokeRoleToMemberListHandler : IRequestHandler<RevokeRoleToMemberListCommand, Result>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public RevokeRoleToMemberListHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public Task<Result> Handle(RevokeRoleToMemberListCommand aRequest, CancellationToken aCancellationToken)
        {
            Task<Result> task = Task.Run(async () => await _mandtrilDiscordBot.RevokeRoleToUserList(aRequest.RoleId, aRequest.FullDiscordHandleList));
            return Task.FromResult(task.Result);

        }

    }
}

