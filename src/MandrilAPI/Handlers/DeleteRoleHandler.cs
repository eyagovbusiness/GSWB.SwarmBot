using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, Result>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public DeleteRoleHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result> Handle(DeleteRoleCommand aRequest, CancellationToken aCancellationToken)
        {
            return (await _mandtrilDiscordBot.DeleteRole(aRequest.RoleId, aCancellationToken));
        }

    }
}

