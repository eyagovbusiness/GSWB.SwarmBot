using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public DeleteCategoryHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result> Handle(DeleteCategoryCommand aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.DeleteCategoryFromId(aRequest.CategoryId, aCancellationToken);

        }

    }
}

