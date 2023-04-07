using MediatR;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Queries
{
    public class GetNumberOfOnlineMembersQuery : IRequest<IResult<int>>
    {
        public GetNumberOfOnlineMembersQuery()
        {
        }

    }
}
