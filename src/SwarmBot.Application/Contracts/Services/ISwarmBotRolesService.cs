﻿using DSharpPlus.Entities;
using Common.Application.DTOs.Discord;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;

namespace SwarmBot.Application
{
    /// <summary>
    /// Public interface of Roles controller that gives access to all the public operations related with Discord Roles.
    /// </summary>
    public interface ISwarmBotRolesService
    {

        /// <summary>
        /// Commands this discord bot get all the available roles in the guild's server.
        /// </summary>
        /// <returns>List of <see cref="DiscordRoleDTO"/> with all the available roles in the guild's server.</returns>
        public Task<IHttpResult<DiscordRoleDTO[]>> GetGuildServerRoleList(ulong guildId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to a given member server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the member.</param>
        /// <param name="aFullDiscordHandle">string representing the full discord Handle with format {MemberName}#{Discriminator} of the member.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public Task<IHttpResult<Unit>> AssignRoleToMember(ulong guildId, ulong aRoleId, string aFullDiscordHandle, string aReason = default!, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every member in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the members.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {MemberName}#{Discriminator} of the members.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong guildId, ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every member in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the members.</param>
        /// <param name="aDiscordMemberList">List of <see cref="DiscordMember"/>.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong guildId, ulong aRoleId, IEnumerable<DiscordMember> aDiscordMemberList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every member in the given member id list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the members.</param>
        /// <param name="aMemberIdList"></param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong guildId, ulong aRoleId, ulong[] aMemberIdList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to revoke a given Discord Role to every member in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to revoke in this server to the members.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {MemberName}#{Discriminator} of the members.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong guildId, ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to revoke a given Discord Role to every member in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to revoke in this server to the members.</param>
        /// <param name="aDiscordMemberList">List of <see cref="DiscordMember"/>.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong guildId, ulong aRoleId, IEnumerable<DiscordMember> aDiscordMemberList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to revoke a given Discord Role to every member in the given member id list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to revoke in this server to the members.</param>
        /// <param name="aMemberIdList">Array of string representing the full discord Handle with format {MemberName}#{Discriminator} of the members.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong guildId, ulong aRoleId, ulong[] aMemberIdList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to create a new Role in the context server.
        /// </summary>
        /// <param name="aRoleName">string that will name the new Role.</param>
        /// <returns><see cref="IHttpResult{ulong}"/> with information about success or fail on this operation and the Id of the new Role if succeed.</returns>
        public Task<IHttpResult<ulong>> CreateRole(ulong guildId, string aRoleName, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to create the GuildSwarmAdmin Role in the context server and return the updated list of roles after that.
        /// </summary>
        /// <returns>List the updated list of roles after the new role creation if succeed.</returns>
        public Task<IHttpResult<IEnumerable<DiscordRoleDTO>>> CreateGuildSwarmAdminRole(ulong guildId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to delete a given Role in the context server.
        /// </summary>
        /// <param name="aRoleId">string that represents the name the Role to delete.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public Task<IHttpResult<Unit>> DeleteRole(ulong guildId, ulong aRoleId, CancellationToken aCancellationToken = default);

    }
}
