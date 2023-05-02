using MandrilAPI.Commands;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class CreateCategoryFromTemplateHandler : IRequestHandler<CreateCategoryFromTemplateCommand, IResult<string>>
    {
        private readonly IChannelsController _channelsController;
        public CreateCategoryFromTemplateHandler(IChannelsController aChannelsController)
            => _channelsController = aChannelsController;

        public async Task<IResult<string>> Handle(CreateCategoryFromTemplateCommand aRequest, CancellationToken aCancellationToken)
            => await _channelsController.CreateCategoryFromTemplate(aRequest.CategoryChannelTemplate, aCancellationToken);

    }
}

