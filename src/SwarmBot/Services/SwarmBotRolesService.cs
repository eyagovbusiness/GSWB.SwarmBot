﻿using DSharpPlus.Entities;
using SwarmBot.Application;
using Common.Application.DTOs.Discord;
using SwarmBot.Application.Mapping;
using SwarmBot.Handelers;
using SwarmBot.Handlers;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;
using Common.Domain.Validation;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace SwarmBot.Services
{
    /// <summary>
    /// SwarmBot bot service that gives support to DiscordRole related operations.
    /// </summary>
    /// <remarks>Depends on <see cref="ISwarmBotDiscordBot"/>.</remarks>
    public partial class SwarmBotRolesService(ISwarmBotDiscordBot aSwarmBotDiscordBot) : ISwarmBotRolesService
    {

        private readonly GuildsHandler _guildsHandler = new(aSwarmBotDiscordBot);

        public async Task<IHttpResult<DiscordRoleDTO[]>> GetGuildServerRoleList(ulong guildId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                .Map(discordGuild => discordGuild.Roles
                                    .Select(rolePair => rolePair.Value)
                                    .Select(role => role.ToDto(guildId)).ToArray())
                .Verify(roleList => roleList != null && roleList.Length > 0, DiscordBotErrors.Role.GuildRolesFetchFailed);

        public async Task<IHttpResult<Unit>> AssignRoleToMember(ulong guildId, ulong aRoleId, string aFullDiscordHandle, string aReason = null!, CancellationToken aCancellationToken = default)
        {
            DiscordGuild lDiscordGuild = default!; DiscordRole lDiscordRole = default!;
            return await MembersHandler.ValidateMemberHandle(aFullDiscordHandle)
                        .Bind(_ => _guildsHandler.GetGuildById(guildId, true, aCancellationToken))
                        .Tap(discordGuild => lDiscordGuild = discordGuild)
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken))
                        .Tap(discordRole => lDiscordRole = discordRole)
                        .Bind(discordGuild => MembersHandler.GetDiscordMemberAtmAsync(lDiscordGuild, aFullDiscordHandle, aCancellationToken))
                        .Bind(discordMember => RolesHandler.GrantRoleToMemberAtmAsync(discordMember, lDiscordRole, aReason, aCancellationToken));

        }

        public async Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong guildId, ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default!;
            return await MembersHandler.ValidateMemberHandleList(aFullHandleList)
                        .Bind(_ => _guildsHandler.GetGuildById(guildId, true, aCancellationToken))
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                            .Tap(discordRole => lDiscordRole = discordRole)
                            .Bind(_ => MembersHandler.GetDiscordMemberListAtmAsync(discordGuild, aFullHandleList, aCancellationToken))
                            .Bind(discordMemberList => RolesHandler.GrantRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));

        }

        public async Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong guildId, ulong aRoleId, IEnumerable<DiscordMember> aDiscordMemberList, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken))
                .Bind(discordRole => RolesHandler.GrantRoleToMemberListAtmAsync(aDiscordMemberList, discordRole));

        public async Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong guildId, ulong aRoleId, ulong[] aMemberIdList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default!;
            return await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                        .Tap(discordRole => lDiscordRole = discordRole)
                        .Bind(_ => MembersHandler.GetDiscordMemberList(discordGuild, member => aMemberIdList.Contains(member.Id), aCancellationToken))
                        .Bind(discordMemberList => RolesHandler.GrantRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));

        }

        public async Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong guildId, ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default!;
            return await MembersHandler.ValidateMemberHandleList(aFullHandleList)
                        .Bind(_ => _guildsHandler.GetGuildById(guildId, true, aCancellationToken))
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                        .Tap(discordRole => lDiscordRole = discordRole)
                        .Bind(_ => MembersHandler.GetDiscordMemberListAtmAsync(discordGuild, aFullHandleList, aCancellationToken))
                        .Bind(discordMemberList => RolesHandler.RevokeRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));

        }

        public async Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong guildId, ulong aRoleId, IEnumerable<DiscordMember> aDiscordMemberList, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken))
                .Bind(discordRole => RolesHandler.RevokeRoleToMemberListAtmAsync(aDiscordMemberList, discordRole));

        public async Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong guildId,ulong aRoleId, ulong[] aMemberIdList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default!;
            return await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                        .Tap(discordRole => lDiscordRole = discordRole)
                        .Bind(_ => MembersHandler.GetDiscordMemberList(discordGuild, member => aMemberIdList.Contains(member.Id), aCancellationToken))
                        .Bind(discordMemberList => RolesHandler.RevokeRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));
        }

        public async Task<IHttpResult<ulong>> CreateRole(ulong guildId, string aRoleName, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                    .Bind(discordGuild => RolesHandler.CreateRoleAtmAsync(discordGuild, aRoleName, aCancellationToken));
        public async Task<IHttpResult<IEnumerable<DiscordRoleDTO>>> CreateGuildSwarmAdminRole(ulong guildId, CancellationToken aCancellationToken = default)
        {
            DiscordGuild discordGuild = default!;
            return await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                .Tap(guild => discordGuild = guild)
                .Bind(discordGuild => RolesHandler.CreateRoleAtmAsync(discordGuild, InvariantConstants.Role_Name_IsGuildSwarmAdmin, aCancellationToken))
                .Map(_ => discordGuild.Roles
                    .Select(rolePair => rolePair.Value)
                    .Select(role => role.ToDto(guildId)).ToArray() as IEnumerable<DiscordRoleDTO>
                );

        }
        public async Task<IHttpResult<Unit>> DeleteRole(ulong guildId, ulong aRoleId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetGuildById(guildId, true, aCancellationToken)
                    .Bind(discordGuild => RolesHandler.DeleteRoleAtmAsync(discordGuild, aRoleId, aCancellationToken));

    }
}
