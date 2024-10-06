using Common.Application.DTOs.Guilds;
using TGF.Common.ROP.HttpResult;

namespace SwarmBot.Application
{
    public interface ISwarmBotUsersService
    {
        /// <summary>
        /// Commands this discord bot to get if exist a user account with the given Id.
        /// </summary>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns><see cref="IHttpResult{bool}"/> with true if an user was found with the given Id, false otherwise and information about success or failure on this operation.</returns>
        public Task<IHttpResult<bool>> ExistUser(ulong aUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to get if a given user has verified account. 
        /// </summary>
        /// <returns><see cref="IHttpResult{bool}"/> with true if the user has verified account, false otherwise and information about success or failure on this operation.</returns>
        public Task<IHttpResult<bool>> IsUserVerified(ulong aUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to get the date of creation of the given user account.
        /// </summary>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns><see cref="IHttpResult{DateTimeOffset}"/> with the date of creation of the given user account and information about success or failure on this operation.</returns>
        public Task<IHttpResult<DateTimeOffset>> GetUserCreationDate(ulong aUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to get the list of guild where both the bot and the user under the provided id are in.
        /// </summary>
        /// <param name="userId">Id of the User.</param>
        /// <returns><see cref="IHttpResult{GuildDTO}"/> with the list of guild where both the bot and the user under the provided id are in.</returns>
        public Task<IHttpResult<IEnumerable<GuildDTO>>> GetUserGuilds(ulong userId, CancellationToken cancellationToken = default);
    }
}
