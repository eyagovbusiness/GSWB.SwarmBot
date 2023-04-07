using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Queries
{
    public class GetExistingCategoryIdQuery : IRequest<IResult<string>>
    {
        public string CategoryName { get; private set; }
        public GetExistingCategoryIdQuery(string aCategoryName)
        {
            CategoryName = aCategoryName;
        }

    }
}
