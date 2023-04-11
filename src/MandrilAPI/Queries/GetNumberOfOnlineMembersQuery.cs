using MediatR;
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
