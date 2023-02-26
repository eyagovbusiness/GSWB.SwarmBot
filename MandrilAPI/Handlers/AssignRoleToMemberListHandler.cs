using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class AssignRoleToMemberListHandler : IRequestHandler<AssignRoleToMemberListCommand, Result>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public AssignRoleToMemberListHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result> Handle(AssignRoleToMemberListCommand aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.AssignRoleToMemberList(aRequest.RoleId, aRequest.FullDiscordIdentifierList, aCancellationToken);

        }

    }
}

