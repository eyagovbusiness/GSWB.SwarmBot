using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Result<string>>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public CreateRoleHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public Task<Result<string>> Handle(CreateRoleCommand aRequest, CancellationToken aCancellationToken)
        {
            Task<Result<ulong>> task = Task.Run(async () => await _mandtrilDiscordBot.CreateRole(aRequest.RoleName));
            return Task.FromResult(task.Result.GetConverted<string>());

        }

    }
}

