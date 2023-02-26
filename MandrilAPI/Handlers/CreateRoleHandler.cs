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

        public async Task<Result<string>> Handle(CreateRoleCommand aRequest, CancellationToken aCancellationToken)
        {
            return (await _mandtrilDiscordBot.CreateRole(aRequest.RoleName, aCancellationToken)).ToString();
        }

    }
}

