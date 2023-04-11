using MandrilBot;
using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Commands
{
    public class CreateCategoryFromTemplateCommand : IRequest<IResult<string>>
    {
        public CategoryChannelTemplate CategoryChannelTemplate { get; private set; }

        public CreateCategoryFromTemplateCommand(CategoryChannelTemplate aCategoryChannelTemplate)
        {
            CategoryChannelTemplate = aCategoryChannelTemplate;
        }

    }
}
