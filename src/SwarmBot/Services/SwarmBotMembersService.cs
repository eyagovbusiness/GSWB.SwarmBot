using DSharpPlus.Entities;
using SwarmBot.Application;
using Common.Application.DTOs.Discord;
using SwarmBot.Application.Mapping;
using SwarmBot.Extensions;
using SwarmBot.Handelers;
using TGF.Common.ROP.HttpResult;
using SwarmBot.Handlers;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace SwarmBot.Services
{
    /// <summary>
    /// SwarmBot bot service that gives support to DiscordMember related operations.
    /// </summary>
    /// <remarks>Depends on <see cref="ISwarmBotDiscordBot"/>.</remarks>
    public class SwarmBotMembersService : ISwarmBotMembersService
    {
        private readonly GuildsHandler _guildsHandler;
        public SwarmBotMembersService(ISwarmBotDiscordBot aSwarmBotDiscordBot)
            => _guildsHandler = new GuildsHandler(aSwarmBotDiscordBot);

        #region ISwarmBotMembersService

        public async Task<IHttpResult<int>> GetNumberOfOnlineMembers(ulong guildId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                    .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(discordMemberList => discordMemberList.Count(x => x.Presence != null
                                                                            && x.Presence.Status == UserStatus.Online
                                                                            && x.VoiceState?.Channel != null));
        public async Task<IHttpResult<DiscordProfileDTO>> GetMemberProfileFromId(ulong guildId, ulong aDiscordUserId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                    .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(discordMemberList => discordMemberList.FirstOrDefault(member => member.Id == aDiscordUserId))
                    .Map(discordMember => new DiscordProfileDTO(discordMember?.DisplayName!, discordMember!.GetGuildAvatarUrlOrDefault()));

        public async Task<IHttpResult<DiscordMember>> GetTester(ulong guildId, ulong aDiscordUserId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                    .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(discordMemberList => discordMemberList.FirstOrDefault(member => member.Id == aDiscordUserId)!)
                    .Verify(discordMember => discordMember != null && discordMember.Roles.Any(role => role.Name == "Tester"), DiscordBotErrors.User.NotTester);

        public async Task<IHttpResult<IEnumerable<DiscordMember>>> GetMemberList(ulong guildId, Func<DiscordMember, bool> aFilterFunc, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                    .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(allMemberList => allMemberList.Where(member => aFilterFunc(member)));

        public async Task<IHttpResult<DiscordRoleDTO[]>> GetMemberRoleList(ulong guildId, ulong userId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                .Bind(discordGuild => MembersHandler.GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                .Map(allMemberList => allMemberList.FirstOrDefault(member => member.Id == userId))
                .Verify(member => member is not null, DiscordBotErrors.Member.NotFoundId)
                .Verify(member => member!.Roles.Any(), DiscordBotErrors.Member.NotFoundAnyRole)
                .Map(existingMember => existingMember!.Roles
                                        .Select(role => role.ToDto(guildId))
                                        .OrderByDescending(role => role.Position).ToArray());

        #endregion 

    }
}
