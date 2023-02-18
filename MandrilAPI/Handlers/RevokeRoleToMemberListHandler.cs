using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class RevokeRoleToUserHandler : IRequestHandler<RevokeRoleToUserCommand, Result>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public RevokeRoleToUserHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public Task<Result> Handle(RevokeRoleToUserCommand aRequest, CancellationToken aCancellationToken)
        {
            Task<Result> task = Task.Run(async () => await _mandtrilDiscordBot.RevokeRoleToUserList(aRequest.RoleId, new string[] { aRequest.FullDiscordIdentifier }));
            return Task.FromResult(task.Result);

        }

    }
}

