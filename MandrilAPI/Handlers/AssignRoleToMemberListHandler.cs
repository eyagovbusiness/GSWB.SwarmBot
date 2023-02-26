using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class AssignRoleToMemberListHandler : IRequestHandler<AssignRoleToMemberListCommand, Result>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public AssignRoleToMemberListHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result> Handle(AssignRoleToMemberListCommand aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.AssignRoleToMemberList(aRequest.RoleId, aRequest.FullDiscordIdentifierList, aCancellationToken);

        }

    }
}

