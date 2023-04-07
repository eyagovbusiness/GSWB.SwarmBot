using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, IResult<string>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public CreateRoleHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<string>> Handle(CreateRoleCommand aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.CreateRole(aRequest.RoleName, aCancellationToken);

    }
}

