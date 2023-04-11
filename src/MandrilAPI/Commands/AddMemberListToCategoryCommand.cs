using MediatR;
using TGF.Common.ROP.Result;

namespace MandrilAPI.Commands
{
    public class AddMemberListToCategoryCommand : IRequest<IResult<Unit>>
    {
        public ulong CategoryId { get; private set; }
        public string[] UserFullHandleList { get; private set; }

        public AddMemberListToCategoryCommand(ulong aCategoryId, string[] aUserFullHandleList)
        {
            CategoryId = aCategoryId;
            UserFullHandleList = aUserFullHandleList;
        }

    }
}
