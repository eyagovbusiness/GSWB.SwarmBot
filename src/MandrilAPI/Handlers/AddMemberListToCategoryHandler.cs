using MandrilAPI.Commands;
using MandrilBot.Controllers;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class AddMemberListToCategoryHandler : IRequestHandler<AddMemberListToCategoryCommand, IResult<Unit>>
    {
        private readonly IChannelsController _channelsController;
        public AddMemberListToCategoryHandler(IChannelsController aChannelsController)
            => _channelsController = aChannelsController;

        public async Task<IResult<Unit>> Handle(AddMemberListToCategoryCommand aRequest, CancellationToken aCancellationToken)
            => await _channelsController.AddMemberListToChannel(aRequest.CategoryId, aRequest.UserFullHandleList, aCancellationToken);

    }
}

