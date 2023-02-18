using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class AssignRoleToUserHandler : IRequestHandler<AssignRoleToUserCommand, Result>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public AssignRoleToUserHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public Task<Result> Handle(AssignRoleToUserCommand aRequest, CancellationToken aCancellationToken)
        {
            Task<Result> task = Task.Run(async () => await _mandtrilDiscordBot.AssignRoleToUser(aRequest.RoleId, aRequest.FullDiscordIdentifier));
            return Task.FromResult(task.Result);

        }

    }
}

