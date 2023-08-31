using DSharpPlus.Entities;
using Mandril.Application;
using Mandril.Application.DTOs;
using MandrilBot.Handelers;
using TGF.Common.Extensions;
using TGF.Common.ROP.HttpResult;

namespace MandrilBot.Services
{
    /// <summary>
    /// Mandril bot service that gives support to DiscordMember related operations.
    /// </summary>
    /// <remarks>Depends on <see cref="IMandrilDiscordBot"/>.</remarks>
    public class MandrilMembersService : IMandrilMembersService
    {
        private readonly GuildsHandler _guildsHandler;
        public MandrilMembersService(IMandrilDiscordBot aMandrilDiscordBot)
            => _guildsHandler = new GuildsHandler(aMandrilDiscordBot);

        /// <summary>
        /// Gets the number of total members connected at this moment in the guild server.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="IHttpResult{int}"/> with the number of connected members.</returns>
        public async Task<IHttpResult<int>> GetNumberOfOnlineMembers(CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(discordMemberList => discordMemberList.Count(x => x.Presence != null
                                                                            && x.Presence.Status == UserStatus.Online
                                                                            && x.VoiceState?.Channel != null));

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
        public async Task<IHttpResult<DiscordRoleDTO>> GetMemberHighestRole(ulong aDiscordUserId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                .Map(allMemberList => allMemberList.FirstOrDefault(member => member.Id == aDiscordUserId))
                .Verify(member => member is not null, DiscordBotErrors.Member.NotFoundId)
                .Map(existingMember => existingMember.Roles
                                        .OrderByDescending(role => role.Position)
                                        .FirstOrDefault())
                .Verify(highestRole => highestRole != null && !highestRole.Name.IsNullOrWhiteSpace(), DiscordBotErrors.Member.NotFoundAnyRole)
                .Map(highestRole => new DiscordRoleDTO(highestRole.Id, highestRole.Name, (byte)highestRole.Position));

    }
}
