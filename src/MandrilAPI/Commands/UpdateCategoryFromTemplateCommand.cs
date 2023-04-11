using MandrilBot;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Commands
{
    public class UpdateCategoryFromTemplateCommand : IRequest<IResult<Unit>>
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
