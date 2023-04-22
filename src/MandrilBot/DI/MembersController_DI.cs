using DSharpPlus.Entities;
using MandrilBot.Handelers;
using TGF.Common.ROP.HttpResult;

namespace MandrilBot.Controllers
{
    /// <summary>
    /// Class that contains the logic to perform all the public operations related with Discord Guild.
    /// </summary>
    public partial class MembersController : IMembersController
    {
        private readonly GuildsHandler _guildsHandler;
        public MembersController(IMandrilDiscordBot aMandrilDiscordBot)
            => _guildsHandler = new GuildsHandler(aMandrilDiscordBot);

    }

    /// <summary>
    /// Public interface of Guild controller that gives access to all the public operations related with Discord Guild.
    /// </summary>
    public interface IMembersController
    {

        public Task<IHttpResult<IEnumerable<DiscordMember>>> GetMemberList(Func<DiscordMember, bool> aFilterFunc, CancellationToken aCancellationToken = default);
    }
}
