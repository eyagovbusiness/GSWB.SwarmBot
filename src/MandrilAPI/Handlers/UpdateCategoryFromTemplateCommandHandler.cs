using MandrilAPI.Commands;
using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Handlers
{
    public class UpdateCategoryFromTemplateCommandHandler : IRequestHandler<UpdateCategoryFromTemplateCommand, IResult<Unit>>
    {
        private readonly IMandrilDiscordBot _mandtrilDiscordBot;
        public UpdateCategoryFromTemplateCommandHandler(IMandrilDiscordBot aMandrilDiscordBot)
            => _mandtrilDiscordBot = aMandrilDiscordBot;

        public async Task<IResult<Unit>> Handle(UpdateCategoryFromTemplateCommand aRequest, CancellationToken aCancellationToken)
            => await _mandtrilDiscordBot.SyncExistingCategoryWithTemplate(aRequest.CategoryId, aRequest.CategoryChannelTemplate, aCancellationToken);

    }
}

