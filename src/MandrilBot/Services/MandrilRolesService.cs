using DSharpPlus.Entities;
using Mandril.Application;
using Mandril.Application.DTOs;
using MandrilBot.Handelers;
using MandrilBot.Handlers;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilBot.Services
{
    /// <summary>
    /// Mandril bot service that gives support to DiscordRole related operations.
    /// </summary>
    /// <remarks>Depends on <see cref="IMandrilDiscordBot"/>.</remarks>
    public partial class MandrilRolesService : IMandrilRolesService
    {

        private readonly GuildsHandler _guildsHandler;
        public MandrilRolesService(IMandrilDiscordBot aMandrilDiscordBot)
            => _guildsHandler = new GuildsHandler(aMandrilDiscordBot);


        public async Task<IHttpResult<DiscordRoleDTO[]>> GetGuildServerRoleList(CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                .Map(discordGuild => discordGuild.Roles
                                    .Select(rolePair => rolePair.Value)
                                    .Select(role => new DiscordRoleDTO(role.Id, role.Name, (byte)role.Position)).ToArray())
                .Verify(roleList => roleList!= null && roleList.Length > 0, DiscordBotErrors.Role.GuildRolesFetchFailed);

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

        public async Task<IHttpResult<string>> CreateRole(string aRoleName, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => RolesHandler.CreateRoleAtmAsync(discordGuild, aRoleName, aCancellationToken));

        public async Task<IHttpResult<Unit>> DeleteRole(ulong aRoleId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => RolesHandler.DeleteRoleAtmAsync(discordGuild, aRoleId, aCancellationToken));

    }
}
