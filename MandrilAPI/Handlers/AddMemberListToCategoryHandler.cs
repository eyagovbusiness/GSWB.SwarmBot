using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class AddMemberListToCategoryHandler : IRequestHandler<AddMemberListToCategoryCommand, Result>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public AddMemberListToCategoryHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result> Handle(AddMemberListToCategoryCommand aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.AddMemberListToChannel(aRequest.CategoryId, aRequest.UserFullHandleList, aCancellationToken);
        }

    }
}

