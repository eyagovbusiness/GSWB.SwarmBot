using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class CreateCategoryFromTemplateHandler : IRequestHandler<CreateCategoryFromTemplateCommand, Result<string>>
    {
        private readonly MandrilDiscordBot _mandtrilDiscordBot;
        public CreateCategoryFromTemplateHandler(MandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public Task<Result<string>> Handle(CreateCategoryFromTemplateCommand aRequest, CancellationToken aCancellationToken)
        {
            Task<Result<ulong>> task = Task.Run(async () => await _mandtrilDiscordBot.CreateCategoryFromTemplate(aRequest.CategoryChannelTemplate));
            return Task.FromResult(task.Result.GetConverted<string>());

        }

    }
}

