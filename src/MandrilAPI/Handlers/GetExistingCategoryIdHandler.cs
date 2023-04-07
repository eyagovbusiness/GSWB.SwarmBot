using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class GetExistingCategoryIdHandler : IRequestHandler<GetExistingCategoryIdQuery, IResult<string>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public GetExistingCategoryIdHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<string>> Handle(GetExistingCategoryIdQuery aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.GetExistingCategoryId(aRequest.CategoryName);

    }
}

