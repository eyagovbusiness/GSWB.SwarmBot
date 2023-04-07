using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.HttpResult;
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
