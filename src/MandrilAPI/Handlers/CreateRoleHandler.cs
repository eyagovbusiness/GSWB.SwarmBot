using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Result<string>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public CreateRoleHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result<string>> Handle(CreateRoleCommand aRequest, CancellationToken aCancellationToken)
        {
            return (await _mandtrilDiscordBot.CreateRole(aRequest.RoleName, aCancellationToken));
        }

    }
}

