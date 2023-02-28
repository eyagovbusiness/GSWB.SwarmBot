using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class AssignRoleToMemberHandler : IRequestHandler<AssignRoleToMemberCommand, Result>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public AssignRoleToMemberHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result> Handle(AssignRoleToMemberCommand aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.AssignRoleToMember(aRequest.RoleId, aRequest.FullDiscordIdentifier, aCancellationToken: aCancellationToken);
        }

    }
}

