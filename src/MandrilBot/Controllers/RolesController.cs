using DSharpPlus.Entities;
using MandrilBot.Handelers;
using MandrilBot.Handlers;
using MediatR;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilBot.Controllers
{
    public partial class RolesController
    {

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to a given member server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the member.</param>
        /// <param name="aFullDiscordHandle">string representing the full discord Handle with format {membername}#{Discriminator} of the member.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
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

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every member in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the members.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {membername}#{Discriminator} of the members.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default;
            return await MembersHandler.ValidateMemberHandleList(aFullHandleList)
                        .Bind(_ => _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken))
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                        .Tap(discordRole => lDiscordRole = discordRole)
                            .Bind(_ => MembersHandler.GetDiscordMemberListAtmAsync(discordGuild, aFullHandleList, aCancellationToken))
                            .Bind(discordMemberList => RolesHandler.GrantRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));

        }

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every member in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the members.</param>
        /// <param name="aDiscordMemberList">List of <see cref="DiscordMember"/>.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong aRoleId, IEnumerable<DiscordMember> aDiscordMemberList, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken))
                .Bind(discordRole => RolesHandler.GrantRoleToMemberListAtmAsync(aDiscordMemberList, discordRole));

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every member in the given member id list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the members.</param>
        /// <param name="aMemberIdList"></param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong aRoleId, ulong[] aMemberIdList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default;
            return await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                        .Tap(discordRole => lDiscordRole = discordRole)
                        .Bind(_ => MembersHandler.GetDiscordMemberList(discordGuild, member => aMemberIdList.Contains(member.Id), aCancellationToken))
                        .Bind(discordMemberList => RolesHandler.GrantRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));

        }

        /// <summary>
        /// Commands this discord bot to revoke a given Discord Role to every member in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to revoke in this server to the members.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {membername}#{Discriminator} of the members.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
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

        /// <summary>
        /// Commands this discord bot to revoke a given Discord Role to every member in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to revoke in this server to the members.</param>
        /// <param name="aDiscordMemberList">List of <see cref="DiscordMember"/>.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong aRoleId, IEnumerable<DiscordMember> aDiscordMemberList, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken))
                .Bind(discordRole => RolesHandler.RevokeRoleToMemberListAtmAsync(aDiscordMemberList, discordRole));

        /// <summary>
        /// Commands this discord bot to revoke a given Discord Role to every member in the given member id list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to revoke in this server to the members.</param>
        /// <param name="aMemberIdList">Array of string representing the full discord Handle with format {membername}#{Discriminator} of the members.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong aRoleId, ulong[] aMemberIdList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default;
            return await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                        .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                        .Tap(discordRole => lDiscordRole = discordRole)
                        .Bind(_ => MembersHandler.GetDiscordMemberList(discordGuild, member => aMemberIdList.Contains(member.Id), aCancellationToken))
                        .Bind(discordMemberList => RolesHandler.RevokeRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));
        }

        /// <summary>
        /// Commands this discord bot to create a new Role in the context server.
        /// </summary>
        /// <param name="aRoleName">string that will name the new Role.</param>
        /// <returns><see cref="IHttpResult{string}"/> with information about success or fail on this operation and the Id of the new Role if succeed.</returns>
        public async Task<IHttpResult<string>> CreateRole(string aRoleName, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => RolesHandler.CreateRoleAtmAsync(discordGuild, aRoleName, aCancellationToken));

        /// <summary>
        /// Commands this discord bot to delete a given Role in the context server.
        /// </summary>
        /// <param name="aRoleId">string that represents the name the Role to delete.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> DeleteRole(ulong aRoleId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => RolesHandler.DeleteRoleAtmAsync(discordGuild, aRoleId, aCancellationToken));

    }
}
