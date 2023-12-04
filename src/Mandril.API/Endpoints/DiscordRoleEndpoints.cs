using Mandril.Application;
using Mandril.Application.DTOs;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.HttpResult;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordRoleEndpoints : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(MandrilApiRoutes.roles_serverRoles, Get_GuildServerRoles).SetResponseMetadata<DiscordRoleDTO[]>(200);
            aWebApplication.MapPost(MandrilApiRoutes.roles_create, Post_CreateRole).SetResponseMetadata<ulong>(200, 404);
            aWebApplication.MapDelete(MandrilApiRoutes.roles_delete, Delete_Role).SetResponseMetadata(200, 404);
            aWebApplication.MapPut(MandrilApiRoutes.roles_assignToMemberList, Post_AssignRoleToMemberList).SetResponseMetadata(200, 404);
            aWebApplication.MapPut(MandrilApiRoutes.roles_revokeForMemberList, Put_RevokeRoleToMemberList).SetResponseMetadata(200, 404);
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
        private async Task<IResult> Get_GuildServerRoles(IMandrilRolesService aMandrilRolesService, CancellationToken aCancellationToken = default)
            => await aMandrilRolesService.GetGuildServerRoleList(aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Creates a new role in the discord's guild and response with the Id of the new Discord role created.
        /// </summary>
        private async Task<IResult> Post_CreateRole(string roleName, IMandrilRolesService aMandrilRolesService, CancellationToken aCancellationToken = default)
            => await aMandrilRolesService.CreateRole(roleName, aCancellationToken)
            .Map(roleId => roleId.ToString())
            .ToIResult();

        /// <summary>
        /// Delete a given discord guild's role by its Id.
        /// </summary>
        private async Task<IResult> Delete_Role(ulong roleId, IMandrilRolesService aMandrilRolesService, CancellationToken aCancellationToken = default)
            => await aMandrilRolesService.DeleteRole(roleId, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Assign the given role by its Id to the given list of discord guild's members.
        /// </summary>
        private async Task<IResult> Post_AssignRoleToMemberList(ulong roleId, string[] aDiscordHandleList, IMandrilRolesService aMandrilRolesService, CancellationToken aCancellationToken = default)
            => await aMandrilRolesService.AssignRoleToMemberList(roleId, aDiscordHandleList, aCancellationToken: aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Revoke the given role by its Id to the given list of discord guild's members.
        /// </summary>
        private async Task<IResult> Put_RevokeRoleToMemberList(ulong roleId, string[] aDiscordHandleList, IMandrilRolesService aMandrilRolesService, CancellationToken aCancellationToken = default)
            => await aMandrilRolesService.RevokeRoleToMemberList(roleId, aDiscordHandleList, aCancellationToken: aCancellationToken)
            .ToIResult();

        #endregion

    }
}
