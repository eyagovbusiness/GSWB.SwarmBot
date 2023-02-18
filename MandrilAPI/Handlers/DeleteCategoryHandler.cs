using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public DeleteCategoryHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public Task<Result> Handle(DeleteCategoryCommand aRequest, CancellationToken aCancellationToken)
        {
            Task<Result> task = Task.Run(async () => await _mandtrilDiscordBot.DeleteCategoryFromId(aRequest.CategoryId));
            return Task.FromResult(task.Result);

        }

    }
}

