using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, IResult<Unit>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public DeleteRoleHandler(IMandrilDiscordBot aMandrilDiscordBot)
        => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<Unit>> Handle(DeleteRoleCommand aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.DeleteRole(aRequest.RoleId, aCancellationToken);

    }
}

