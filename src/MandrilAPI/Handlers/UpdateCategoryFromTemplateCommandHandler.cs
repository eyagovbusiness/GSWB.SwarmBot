using MandrilAPI.Commands;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class UpdateCategoryFromTemplateCommandHandler : IRequestHandler<UpdateCategoryFromTemplateCommand, IResult<Unit>>
    {
        private readonly IChannelsController _channelsController;
        public UpdateCategoryFromTemplateCommandHandler(IChannelsController aChannelsController)
            => _channelsController = aChannelsController;

        public async Task<IResult<Unit>> Handle(UpdateCategoryFromTemplateCommand aRequest, CancellationToken aCancellationToken)
            => await _channelsController.SyncExistingCategoryWithTemplate(aRequest.CategoryId, aRequest.CategoryChannelTemplate, aCancellationToken);

    }
}

