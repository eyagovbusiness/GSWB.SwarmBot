using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class AssignRoleToUserListHandler : IRequestHandler<AssignRoleToUserListCommand, Result>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public AssignRoleToUserListHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public Task<Result> Handle(AssignRoleToUserListCommand aRequest, CancellationToken aCancellationToken)
        {
            Task<Result> task = Task.Run(async () => await _mandtrilDiscordBot.AssignRoleToUserList(aRequest.RoleId, aRequest.FullDiscordIdentifierList));
            return Task.FromResult(task.Result);

        }

    }
}

