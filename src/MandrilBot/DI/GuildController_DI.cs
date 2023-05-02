using MandrilBot.Handelers;
using TGF.Common.ROP.HttpResult;

namespace MandrilBot.Controllers
{
    /// <summary>
    /// Class that contains the logic to perform all the public operations related with Discord Guild.
    /// </summary>
    public partial class GuildController : IGuildController
    {
        private readonly GuildsHandler _guildsHandler;
        public GuildController(IMandrilDiscordBot aMandrilDiscordBot)
            => _guildsHandler = new GuildsHandler(aMandrilDiscordBot);

    }

    /// <summary>
    /// Public interface of Guild controller that gives access to all the public operations related with Discord Guild.
    /// </summary>
    public interface IGuildController
    {
        /// <summary>
        /// Gets the number of total members connected at this moment in the guild server.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="IHttpResult{int}"/> with the number of connected members and information about success or failureure on this operation.</returns>
        public Task<IHttpResult<int>> GetNumberOfOnlineMembers(CancellationToken aCancellationToken = default);
    }
}
