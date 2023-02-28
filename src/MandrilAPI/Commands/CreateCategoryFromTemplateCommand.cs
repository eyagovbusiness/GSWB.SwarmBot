using MandrilBot;
using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class CreateCategoryFromTemplateCommand : IRequest<Result<string>>
    {
        public CategoryChannelTemplate CategoryChannelTemplate { get; set; }

        public CreateCategoryFromTemplateCommand(CategoryChannelTemplate aCategoryChannelTemplate)
        {
            CategoryChannelTemplate = aCategoryChannelTemplate;
        }

    }
}
