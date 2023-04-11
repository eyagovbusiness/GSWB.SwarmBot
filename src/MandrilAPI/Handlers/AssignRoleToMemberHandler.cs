using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class AssignRoleToMemberHandler : IRequestHandler<AssignRoleToMemberCommand, IResult<Unit>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public AssignRoleToMemberHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<Unit>> Handle(AssignRoleToMemberCommand aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.AssignRoleToMember(aRequest.RoleId, aRequest.FullDiscordIdentifier, aCancellationToken: aCancellationToken);

    }
}

