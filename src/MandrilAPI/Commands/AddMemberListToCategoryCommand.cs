using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class AddMemberListToCategoryCommand : IRequest<Result>
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
