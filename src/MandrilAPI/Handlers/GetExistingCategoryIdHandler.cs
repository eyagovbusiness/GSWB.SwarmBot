using MandrilAPI.Queries;
using MandrilBot;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class GetExistingCategoryIdHandler : IRequestHandler<GetExistingCategoryIdQuery, IResult<string>>
    {
        private readonly IChannelsController _channelsController;
        public GetExistingCategoryIdHandler(IChannelsController aChannelsController)
            => _channelsController = aChannelsController;

        public async Task<IResult<string>> Handle(GetExistingCategoryIdQuery aRequest, CancellationToken aCancellationToken)
            => await _channelsController.GetExistingCategoryId(aRequest.CategoryName);

    }
}

