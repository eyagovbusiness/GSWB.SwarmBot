using DSharpPlus.Entities;
using MandrilBot.Handelers;
using TGF.Common.ROP.HttpResult;

namespace MandrilBot.Controllers
{
    public partial class MembersController
    {
        /// <summary>
        /// Returns 
        /// </summary>
        /// <param name="aFilterFunc"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns></returns>
        public async Task<IHttpResult<IEnumerable<DiscordMember>>> GetMemberList(Func<DiscordMember, bool> aFilterFunc, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(allMemberList => allMemberList.Where(member => aFilterFunc(member)));

    }
}
