using MandrilAPI.Commands;
using MandrilBot;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, IResult<Unit>>
    {
        private readonly IChannelsController _channelsController;
        public DeleteCategoryHandler(IChannelsController aChannelsController)
            => _channelsController = aChannelsController;

        public async Task<IResult<Unit>> Handle(DeleteCategoryCommand aRequest, CancellationToken aCancellationToken)
            => await _channelsController.DeleteCategoryFromId(aRequest.CategoryId, aCancellationToken);

    }
}

