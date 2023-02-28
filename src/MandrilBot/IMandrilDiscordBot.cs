using TheGoodFramework.CA.Domain.Primitives.Result;

namespace MandrilBot
{
    public interface IMandrilDiscordBot
    {
        Task<Result> AddMemberListToChannel(ulong aChannelId, string[] aUserFullHandleList, CancellationToken aCancellationToken = default);
        Task<Result> AssignRoleToMember(ulong aRoleId, string aFullDiscordHandle, string aReason = null, CancellationToken aCancellationToken = default);
        Task<Result> AssignRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default);
        Task<Result<ulong>> CreateCategoryFromTemplate(CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default);
        Task<Result<ulong>> CreateRole(string aRoleName, CancellationToken aCancellationToken = default);
        Task<Result> DeleteCategoryFromId(ulong aEventCategorylId, CancellationToken aCancellationToken = default);
        Task<Result<bool>> ExistUser(ulong aUserId, CancellationToken aCancellationToken = default);
        Task<Result<int>> GetNumberOfOnlineUsers(CancellationToken aCancellationToken = default);
        Task<Result<DateTimeOffset>> GetUserCreationDate(ulong aUserId, CancellationToken aCancellationToken = default);
        Task<Result<bool>> IsUserVerified(ulong aUserId, CancellationToken aCancellationToken = default);
        Task<Result> RevokeRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default);
        Task StartAsync();
    }
}
