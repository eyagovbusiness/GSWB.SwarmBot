using MediatR;
using TGF.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class AddMemberListToCategoryCommand : IRequest<Result>
    {
        public ulong CategoryId { get; set; }
        public string[] UserFullHandleList { get; set; }

        public AddMemberListToCategoryCommand(ulong aCategoryId, string[] aUserFullHandleList)
        {
            CategoryId = aCategoryId;
            UserFullHandleList = aUserFullHandleList;
        }

    }
}
