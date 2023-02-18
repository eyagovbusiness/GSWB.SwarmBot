using MediatR;
using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilAPI.Commands
{
    public class AddUserListToCategoryCommand : IRequest<Result>
    {
        public ulong CategoryId { get; set; }
        public string[] UserFullHandleList { get; set; }

        public AddUserListToCategoryCommand(ulong aCategoryId, string[] aUserFullHandleList)
        {
            CategoryId = aCategoryId;
            UserFullHandleList = aUserFullHandleList;
        }

    }
}
