using MandrilAPI.Queries;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class GetExistingCategoryIdHandler : IRequestHandler<GetExistingCategoryIdQuery, Result<string>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public GetExistingCategoryIdHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result<string>> Handle(GetExistingCategoryIdQuery aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.GetExistingCategoryId(aRequest.CategoryName);
        }

    }
}

