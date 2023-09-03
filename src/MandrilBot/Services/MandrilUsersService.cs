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


        public async Task<IHttpResult<bool>> ExistUser(ulong aUserId, CancellationToken aCancellationToken = default)
            => await _usersHandler.GetUserAsync(aUserId, aCancellationToken)
                    .Map(discordUser => discordUser != null);

        public async Task<IHttpResult<bool>> IsUserVerified(ulong aUserId, CancellationToken aCancellationToken = default)//TO-DO: WRONG ERROR INFO WHEN INVALID ID
            => await _usersHandler.GetUserAsync(aUserId, aCancellationToken)
                    .Bind(discordUser => Task.FromResult(discordUser != null
                                                         ? Result.SuccessHttp(discordUser.Verified.GetValueOrDefault(false))
                                                         : Result.Failure<bool>(DiscordBotErrors.User.NotFoundId)));
        public async Task<IHttpResult<DateTimeOffset>> GetUserCreationDate(ulong aUserId, CancellationToken aCancellationToken = default)//TO-DO: WRONG ERROR INFO WHEN INVALID ID
            => await _usersHandler.GetUserAsync(aUserId, aCancellationToken)
                    .Bind(discordUser => Task.FromResult(discordUser != null
                                                         ? Result.SuccessHttp(discordUser.CreationTimestamp)
                                                         : Result.Failure<DateTimeOffset>(DiscordBotErrors.User.NotFoundId)));

    }
}
