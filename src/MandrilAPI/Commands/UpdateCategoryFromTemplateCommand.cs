using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class UpdateCategoryFromTemplateCommand : IRequest<Result>
    {
        public CategoryChannelTemplate CategoryChannelTemplate { get; private set; }
        public ulong CategoryId { get; private set; }

        public UpdateCategoryFromTemplateCommand(ulong aCategoryId, CategoryChannelTemplate aCategoryChannelTemplate)
        {
            CategoryId = aCategoryId;
            CategoryChannelTemplate = aCategoryChannelTemplate;
        }

    }
}
