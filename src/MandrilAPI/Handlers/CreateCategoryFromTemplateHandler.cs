using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class CreateCategoryFromTemplateHandler : IRequestHandler<CreateCategoryFromTemplateCommand, IResult<string>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public CreateCategoryFromTemplateHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<string>> Handle(CreateCategoryFromTemplateCommand aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.CreateCategoryFromTemplate(aRequest.CategoryChannelTemplate, aCancellationToken);

    }
}

