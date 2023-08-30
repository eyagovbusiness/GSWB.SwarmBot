using Mandril.Application;
using MandrilBot.Handlers;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilBot.Services
{
    /// <summary>
    /// Mandril bot service that gives support to DiscordUser related operations.
    /// </summary>
    /// <remarks>Depends on <see cref="IMandrilDiscordBot"/>.</remarks>
    public partial class MandrilUsersService : IMandrilUsersService
    {
        private readonly UsersHandler _usersHandler;
        public MandrilUsersService(IMandrilDiscordBot aMandrilDiscordBot)
            => _usersHandler = new UsersHandler(aMandrilDiscordBot);

        /// <summary>
        /// Commands this discord bot to get if exist a user account with the given Id.
        /// </summary>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns><see cref="IHttpResult{bool}"/> with true if an user was found with the given Id, false otherwise and information about success or failure on this operation.</returns>
        public async Task<IHttpResult<bool>> ExistUser(ulong aUserId, CancellationToken aCancellationToken = default)
            => await _usersHandler.GetUserAsync(aUserId, aCancellationToken)
                    .Map(discordUser => discordUser != null);

        /// <summary>
        /// Commands this discord bot to get if a given user has verified account. 
        /// </summary>
        /// <returns><see cref="IHttpResult{bool}"/> with true if the user has verified account, false otherwise and information about success or failure on this operation.</returns>
        public async Task<IHttpResult<bool>> IsUserVerified(ulong aUserId, CancellationToken aCancellationToken = default)//TO-DO: WRONG ERROR WHEN INVALID ID
            => await _usersHandler.GetUserAsync(aUserId, aCancellationToken)
                    .Bind(discordUser => Task.FromResult(discordUser != null
                                                         ? Result.SuccessHttp(discordUser.Verified.GetValueOrDefault(false))
                                                         : Result.Failure<bool>(DiscordBotErrors.User.NotFoundId)));

        /// <summary>
        /// Commands this discord bot to get the date of creation of the given user account.
        /// </summary>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns><see cref="IHttpResult{DateTimeOffset}"/> with the date of creation of the given user account and information about success or failure on this operation.</returns>
        public async Task<IHttpResult<DateTimeOffset>> GetUserCreationDate(ulong aUserId, CancellationToken aCancellationToken = default)//TO-DO: WRONG ERROR WHEN INVALID ID
            => await _usersHandler.GetUserAsync(aUserId, aCancellationToken)
                    .Bind(discordUser => Task.FromResult(discordUser != null
                                                         ? Result.SuccessHttp(discordUser.CreationTimestamp)
                                                         : Result.Failure<DateTimeOffset>(DiscordBotErrors.User.NotFoundId)));


    }
}
