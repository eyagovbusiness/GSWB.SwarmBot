using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class DeleteCategoryCommand : IRequest<Result>
    {
        public ulong CategoryId { get; private set; }

        public DeleteCategoryCommand(ulong aCategoryId)
        {
            CategoryId = aCategoryId;

        }

    }
}
