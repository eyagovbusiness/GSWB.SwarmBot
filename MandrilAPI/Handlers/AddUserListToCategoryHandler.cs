using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class AddUserListToCategoryHandler : IRequestHandler<AddUserListToCategoryCommand, Result>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public AddUserListToCategoryHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public Task<Result> Handle(AddUserListToCategoryCommand aRequest, CancellationToken aCancellationToken)
        {
            Task<Result> task = Task.Run(async () => await _mandtrilDiscordBot.AddUserListToChannel(aRequest.CategoryId, aRequest.UserFullHandleList));
            return Task.FromResult(task.Result);

        }

    }
}

