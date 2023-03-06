using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Handlers
{
    public class UpdateCategoryFromTemplateCommandHandler : IRequestHandler<UpdateCategoryFromTemplateCommand, Result>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public UpdateCategoryFromTemplateCommandHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _mandtrilDiscordBot = aMandrilDiscordBot;
        }

        public async Task<Result> Handle(UpdateCategoryFromTemplateCommand aRequest, CancellationToken aCancellationToken)
        {
            return await _mandtrilDiscordBot.SyncExistingCategoryWithTemplate(aRequest.CategoryId, aRequest.CategoryChannelTemplate, aCancellationToken);
        }

    }
}

