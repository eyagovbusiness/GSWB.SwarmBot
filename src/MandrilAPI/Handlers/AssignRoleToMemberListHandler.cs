using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class AssignRoleToMemberListHandler : IRequestHandler<AssignRoleToMemberListCommand, IResult<Unit>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public AssignRoleToMemberListHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<Unit>> Handle(AssignRoleToMemberListCommand aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.AssignRoleToMemberList(aRequest.RoleId, aRequest.FullDiscordIdentifierList, aCancellationToken);

    }
}

