using DSharpPlus.Entities;
using SwarmBot.Application;
using SwarmBot.Application.DTOs;
using SwarmBot.Application.Mapping;
using SwarmBot.Handelers;
using SwarmBot.Handlers;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace SwarmBot.Services
{
    /// <summary>
    /// SwarmBot bot service that gives support to DiscordRole related operations.
    /// </summary>
    /// <remarks>Depends on <see cref="ISwarmBotDiscordBot"/>.</remarks>
    public partial class SwarmBotRolesService : ISwarmBotRolesService
    {

        private readonly GuildsHandler _guildsHandler;
        public SwarmBotRolesService(ISwarmBotDiscordBot aSwarmBotDiscordBot)
            => _guildsHandler = new GuildsHandler(aSwarmBotDiscordBot);


        public async Task<IHttpResult<DiscordRoleDTO[]>> GetGuildServerRoleList(CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                .Map(discordGuild => discordGuild.Roles
                                    .Select(rolePair => rolePair.Value)
                                    .Select(role => role.ToDto()).ToArray())
                .Verify(roleList => roleList != null && roleList.Length > 0, DiscordBotErrors.Role.GuildRolesFetchFailed);

        public async Task<IHttpResult<Unit>> AssignRoleToMember(ulong aRoleId, string aFullDiscordHandle, string aReason = null, CancellationToken aCancellationToken = default)
        {
            DiscordGuild lDiscordGuild = default; DiscordRole lDiscordRole = default;
            return await MembersHandler.ValidateMemberHandle(aFullDiscordHandle)
                        .Bind(_ => _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken))
                        .Tap(discordGuild => lDiscordGuild = discordGuild)
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken))
                        .Tap(discordRole => lDiscordRole = discordRole)
                        .Bind(discordGuild => MembersHandler.GetDiscordMemberAtmAsync(lDiscordGuild, aFullDiscordHandle, aCancellationToken))
                        .Bind(discordMember => RolesHandler.GrantRoleToMemberAtmAsync(discordMember, lDiscordRole, aReason, aCancellationToken));

        }

        public async Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default!;
            return await MembersHandler.ValidateMemberHandleList(aFullHandleList)
                        .Bind(_ => _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken))
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                            .Tap(discordRole => lDiscordRole = discordRole)
                            .Bind(_ => MembersHandler.GetDiscordMemberListAtmAsync(discordGuild, aFullHandleList, aCancellationToken))
                            .Bind(discordMemberList => RolesHandler.GrantRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));

        }

        public async Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong aRoleId, IEnumerable<DiscordMember> aDiscordMemberList, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken))
                .Bind(discordRole => RolesHandler.GrantRoleToMemberListAtmAsync(aDiscordMemberList, discordRole));

        public async Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong aRoleId, ulong[] aMemberIdList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default!;
            return await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                        .Tap(discordRole => lDiscordRole = discordRole)
                        .Bind(_ => MembersHandler.GetDiscordMemberList(discordGuild, member => aMemberIdList.Contains(member.Id), aCancellationToken))
                        .Bind(discordMemberList => RolesHandler.GrantRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));

        }

        public async Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default;
            return await MembersHandler.ValidateMemberHandleList(aFullHandleList)
                        .Bind(_ => _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken))
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                        .Tap(discordRole => lDiscordRole = discordRole)
                        .Bind(_ => MembersHandler.GetDiscordMemberListAtmAsync(discordGuild, aFullHandleList, aCancellationToken))
                        .Bind(discordMemberList => RolesHandler.RevokeRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));

        }

        public async Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong aRoleId, IEnumerable<DiscordMember> aDiscordMemberList, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken))
                .Bind(discordRole => RolesHandler.RevokeRoleToMemberListAtmAsync(aDiscordMemberList, discordRole));

        public async Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong aRoleId, ulong[] aMemberIdList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default;
            return await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                        .Tap(discordRole => lDiscordRole = discordRole)
                        .Bind(_ => MembersHandler.GetDiscordMemberList(discordGuild, member => aMemberIdList.Contains(member.Id), aCancellationToken))
                        .Bind(discordMemberList => RolesHandler.RevokeRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));
        }

        public async Task<IHttpResult<ulong>> CreateRole(string aRoleName, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => RolesHandler.CreateRoleAtmAsync(discordGuild, aRoleName, aCancellationToken));

        public async Task<IHttpResult<Unit>> DeleteRole(ulong aRoleId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => RolesHandler.DeleteRoleAtmAsync(discordGuild, aRoleId, aCancellationToken));

    }
}
