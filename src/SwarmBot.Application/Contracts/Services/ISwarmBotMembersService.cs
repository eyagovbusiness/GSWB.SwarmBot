using DSharpPlus.Entities;
using Common.Application.DTOs.Discord;
using TGF.Common.ROP.HttpResult;

namespace SwarmBot.Application
{

    /// <summary>
    /// Public interface of Guild controller that gives access to all the public operations related with Discord Guild.
    /// </summary>
    public interface ISwarmBotMembersService
    {

        /// <summary>
        /// Gets the number of total members connected at this moment in the guild server.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="IHttpResult{int}"/> with the number of connected members and information about success or failure on this operation.</returns>
        public Task<IHttpResult<int>> GetNumberOfOnlineMembers(ulong guildId, CancellationToken aCancellationToken = default);

        /// <summary>
        ///  Get the member's basic profile with nickname and AvatarUrl image from the Discord user id.
        /// </summary>
        /// <param name="aDiscordUserId">Discord user id.</param>
        /// <returns><see cref="IHttpResult{DiscordProfileDTO}"/> with the member's discord profile basic info.</returns>
        public Task<IHttpResult<DiscordProfileDTO>> GetMemberProfileFromId(ulong guildId, ulong aDiscordUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Get the tester DiscordMember if exist, otherwise 404. A discord user ID is tester if it is a member of the testers guild and it has the "Tester" role assigned.
        /// </summary>
        /// <param name="aDiscordUserId">Discord user id.</param>
        /// <returns>The DiscordMember who is a Tester or 404 if the provided discord user id is not a tester.</returns>
        public Task<IHttpResult<DiscordMember>> GetTester(ulong guildId, ulong aDiscordUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Returns a list of guild members that satisfied the filter function conditions.
        /// </summary>
        /// <param name="aFilterFunc"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns></returns>
        public Task<IHttpResult<IEnumerable<DiscordMember>>> GetMemberList(ulong guildId, Func<DiscordMember, bool> aFilterFunc, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Returns the highest DiscordRole(from the hierarchy order) assigned to the UserId in the guild.
        /// </summary>
        public Task<IHttpResult<DiscordRoleDTO[]>> GetMemberRoleList(ulong guildId, ulong userId, CancellationToken cancellationToken = default);
    }
}
