using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class CreateCategoryFromTemplateHandler : IRequestHandler<CreateCategoryFromTemplateCommand, Result<string>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public CreateCategoryFromTemplateHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result<string>> Handle(CreateCategoryFromTemplateCommand aRequest, CancellationToken aCancellationToken)
        {
            return (await _mandtrilDiscordBot.CreateCategoryFromTemplate(aRequest.CategoryChannelTemplate, aCancellationToken)).ToString();
        }

    }
}

