using SwarmBot.Application;
using Common.Application.DTOs.Discord;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.HttpResult;
using Common.Infrastructure.Communication.ApiRoutes;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordRoleEndpoints : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(SwarmBotApiRoutes.roles, Get_GuildServerRoles).SetResponseMetadata<DiscordRoleDTO[]>(200);
            aWebApplication.MapPost(SwarmBotApiRoutes.roles, Post_CreateRole).SetResponseMetadata<ulong>(200, 404);
            aWebApplication.MapDelete(SwarmBotApiRoutes.roles, Delete_Role).SetResponseMetadata(200, 404);
            aWebApplication.MapPut(SwarmBotApiRoutes.roles_assign, Post_AssignRoleToMemberList).SetResponseMetadata(200, 404);
            aWebApplication.MapPut(SwarmBotApiRoutes.roles_revoke, Put_RevokeRoleToMemberList).SetResponseMetadata(200, 404);
        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        #endregion

        #region EndpointMethods

        /// <summary>
        /// Gets a list with all the available roles in the guild's server.
        /// </summary>
        private async Task<IResult> Get_GuildServerRoles(ISwarmBotRolesService aSwarmBotRolesService, CancellationToken aCancellationToken = default)
            => await aSwarmBotRolesService.GetGuildServerRoleList(aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Creates a new role in the discord's guild and response with the Id of the new Discord role created.
        /// </summary>
        private async Task<IResult> Post_CreateRole(string name, ISwarmBotRolesService aSwarmBotRolesService, CancellationToken aCancellationToken = default)
            => await aSwarmBotRolesService.CreateRole(name, aCancellationToken)
            .Map(roleId => roleId.ToString())
            .ToIResult();

        /// <summary>
        /// Delete a given discord guild's role by its Id.
        /// </summary>
        private async Task<IResult> Delete_Role(ulong id, ISwarmBotRolesService aSwarmBotRolesService, CancellationToken aCancellationToken = default)
            => await aSwarmBotRolesService.DeleteRole(id, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Assign the given role by its Id to the given list of discord guild's members.
        /// </summary>
        private async Task<IResult> Post_AssignRoleToMemberList(ulong id, string[] aDiscordHandleList, ISwarmBotRolesService aSwarmBotRolesService, CancellationToken aCancellationToken = default)
            => await aSwarmBotRolesService.AssignRoleToMemberList(id, aDiscordHandleList, aCancellationToken: aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Revoke the given role by its Id to the given list of discord guild's members.
        /// </summary>
        private async Task<IResult> Put_RevokeRoleToMemberList(ulong id, string[] aDiscordHandleList, ISwarmBotRolesService aSwarmBotRolesService, CancellationToken aCancellationToken = default)
            => await aSwarmBotRolesService.RevokeRoleToMemberList(id, aDiscordHandleList, aCancellationToken: aCancellationToken)
            .ToIResult();

        #endregion

    }
}
