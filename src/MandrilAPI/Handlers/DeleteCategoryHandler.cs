using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, IResult<Unit>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public DeleteCategoryHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<Unit>> Handle(DeleteCategoryCommand aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.DeleteCategoryFromId(aRequest.CategoryId, aCancellationToken);

    }
}

