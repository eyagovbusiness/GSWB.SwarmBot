using DSharpPlus.Entities;
using MandrilBot.Handelers;
using TGF.Common.Extensions;
using TGF.Common.ROP.HttpResult;

namespace MandrilBot.Controllers
{
    public partial class MembersController
    {
        /// <summary>
        /// Returns a list of guild members that satisfied the filter function conditions.
        /// </summary>
        /// <param name="aFilterFunc"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns></returns>
        public async Task<IHttpResult<IEnumerable<DiscordMember>>> GetMemberList(Func<DiscordMember, bool> aFilterFunc, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(allMemberList => allMemberList.Where(member => aFilterFunc(member)));

        /// <summary>
        /// Returns the highest DiscordRole(from the hierarchy order) assigned to the UserId in the guild.
        /// </summary>
        /// <param name="aDiscordUserId"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns></returns>
        public async Task<IHttpResult<DiscordRole>> GetMemberHighestRole(ulong aDiscordUserId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                .Map(allMemberList => allMemberList.FirstOrDefault(member => member.Id == aDiscordUserId))
                .Verify(member => member is not null, DiscordBotErrors.Member.NotFoundId)
                .Map(existingMember => existingMember.Roles
                                        .OrderByDescending(role => role.Position)
                                        .FirstOrDefault())
                .Verify(highestRole => highestRole != null && !highestRole.Name.IsNullOrWhiteSpace(), DiscordBotErrors.Member.NotFoundAnyRole);

    }
}
