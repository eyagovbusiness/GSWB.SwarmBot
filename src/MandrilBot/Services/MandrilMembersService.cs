using DSharpPlus.Entities;
using Mandril.Application;
using Mandril.Application.DTOs;
using Mandril.Application.Mapping;
using MandrilBot.Extensions;
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

        #region IMandrilMembersService

        public async Task<IHttpResult<int>> GetNumberOfOnlineMembers(CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(discordMemberList => discordMemberList.Count(x => x.Presence != null
                                                                            && x.Presence.Status == UserStatus.Online
                                                                            && x.VoiceState?.Channel != null));
        public async Task<IHttpResult<DiscordProfileDTO>> GetMemberProfileFromId(ulong aDiscordUserId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(discordMemberList => discordMemberList.FirstOrDefault(member => member.Id == aDiscordUserId))
                    .Map(discordMember => new DiscordProfileDTO(discordMember.DisplayName, discordMember.GetGuildAvatarUrlOrDefault()));

        public async Task<IHttpResult<IEnumerable<DiscordMember>>> GetMemberList(Func<DiscordMember, bool> aFilterFunc, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(allMemberList => allMemberList.Where(member => aFilterFunc(member)));

        public async Task<IHttpResult<DiscordRoleDTO[]>> GetMemberRoleList(ulong aDiscordUserId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                .Map(allMemberList => allMemberList.FirstOrDefault(member => member.Id == aDiscordUserId))
                .Verify(member => member is not null, DiscordBotErrors.Member.NotFoundId)
                .Verify(member => member.Roles.Any(), DiscordBotErrors.Member.NotFoundAnyRole)
                .Map(existingMember => existingMember.Roles
                                        .Select(role => role.ToDto())
                                        .OrderByDescending(role => role.Position).ToArray());

        #endregion 

    }
}
