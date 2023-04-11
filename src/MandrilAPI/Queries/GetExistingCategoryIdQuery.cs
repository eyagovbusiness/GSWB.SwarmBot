using MediatR;
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
