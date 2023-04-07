using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class AddMemberListToCategoryHandler : IRequestHandler<AddMemberListToCategoryCommand, IResult<Unit>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public AddMemberListToCategoryHandler(IMandrilDiscordBot aMandrilDiscordBot)
        => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<Unit>> Handle(AddMemberListToCategoryCommand aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.AddMemberListToChannel(aRequest.CategoryId, aRequest.UserFullHandleList, aCancellationToken);

    }
}

