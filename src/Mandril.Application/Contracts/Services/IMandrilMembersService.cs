using DSharpPlus.Entities;
using Mandril.Application.DTOs;
using TGF.Common.ROP.HttpResult;

namespace Mandril.Application
{

    /// <summary>
    /// Public interface of Guild controller that gives access to all the public operations related with Discord Guild.
    /// </summary>
    public interface IMandrilMembersService
    {

        /// <summary>
        /// Gets the number of total members connected at this moment in the guild server.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="IHttpResult{int}"/> with the number of connected members and information about success or failureure on this operation.</returns>
        public Task<IHttpResult<int>> GetNumberOfOnlineMembers(CancellationToken aCancellationToken = default);

        /// <summary>
        /// Returns a list of guild members that satisfied the filter function conditions.
        /// </summary>
        /// <param name="aFilterFunc"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns></returns>
        public Task<IHttpResult<IEnumerable<DiscordMember>>> GetMemberList(Func<DiscordMember, bool> aFilterFunc, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Returns the highest DiscordRole(from the hierarchy order) assigned to the UserId in the guild.
        /// </summary>
        /// <param name="aDiscordUserId"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns></returns>
        public Task<IHttpResult<DiscordRoleDTO>> GetMemberHighestRole(ulong aDiscordUserId, CancellationToken aCancellationToken = default);
    }
}
