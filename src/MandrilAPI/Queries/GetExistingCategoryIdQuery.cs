using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Queries
{
    public class GetExistingCategoryIdQuery : IRequest<Result<string>>
    {
        public string CategoryName { get; private set; }
        public GetExistingCategoryIdQuery(string aCategoryName)
        {
            CategoryName = aCategoryName;
        }

    }
}
