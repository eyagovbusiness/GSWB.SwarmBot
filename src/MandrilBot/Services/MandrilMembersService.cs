using DSharpPlus.Entities;
using Mandril.Application;
using Mandril.Application.DTOs;
using MandrilBot.Handelers;
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
        ///  Get the member's server nickname from the Discord user id.
        /// </summary>
        /// <param name="aDiscordUserId">Discord user id.</param>
        /// <returns><see cref="IHttpResult{string}"/> with the member's server nickname.</returns>
        public async Task<IHttpResult<string>> GetMemberNicknameFromUserId(string aDiscordUserId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(discordMemberList => discordMemberList.FirstOrDefault(member => member.Id == ulong.Parse(aDiscordUserId)).Nickname);

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
        public async Task<IHttpResult<DiscordRoleDTO[]>> GetMemberRoleList(ulong aDiscordUserId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                .Map(allMemberList => allMemberList.FirstOrDefault(member => member.Id == aDiscordUserId))
                .Verify(member => member is not null, DiscordBotErrors.Member.NotFoundId)
                .Verify(member => member.Roles.Any(), DiscordBotErrors.Member.NotFoundAnyRole)
                .Map(existingMember => existingMember.Roles
                                        .Select(role => new DiscordRoleDTO(role.Id, role.Name, (byte)role.Position))
                                        .OrderByDescending(role => role.Position).ToArray());

    }
}
