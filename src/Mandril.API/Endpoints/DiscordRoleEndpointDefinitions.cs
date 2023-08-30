using TGF.CA.Presentation;
using TGF.CA.Presentation.MinimalAPI;
using TGF.CA.Presentation.Middleware;
using Mandril.Application;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordRoleEndpointDefinitions : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapPost("/createRole", PostCreateRole).SetResponseMetadata<string>(200, 404);
            aWebApplication.MapDelete("/deleteRole", DeleteRole).SetResponseMetadata(200, 404);
            aWebApplication.MapPut("/assignRoleToMemberList", PostAssignRoleToMemberList).SetResponseMetadata(200, 404);
            aWebApplication.MapPut("/revokeRoleToMemberList", PutRevokeRoleToMemberList).SetResponseMetadata(200, 404);

        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        #endregion

        #region EndpointMethods

        /// <summary>
        /// Creates a new role in the discord's guild and response with the Id of the new Discord role created.
        /// </summary>
        private async Task<IResult> PostCreateRole(string aNewRoleName, IMandrilRolesService aMandrilRolesService, CancellationToken aCancellationToken = default)
            => await aMandrilRolesService.CreateRole(aNewRoleName, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Delete a given discord guild's role by its Id.
        /// </summary>
        private async Task<IResult> DeleteRole(ulong aRoleId, IMandrilRolesService aMandrilRolesService, CancellationToken aCancellationToken = default)
            => await aMandrilRolesService.DeleteRole(aRoleId, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Assign the given role by its Id to the given list of discord guild's members.
        /// </summary>
        private async Task<IResult> PostAssignRoleToMemberList(ulong aRoleId, string[] aFullDiscordHandleList, IMandrilRolesService aMandrilRolesService, CancellationToken aCancellationToken = default)
            => await aMandrilRolesService.AssignRoleToMemberList(aRoleId, aFullDiscordHandleList, aCancellationToken: aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Revoke the given role by its Id to the given list of discord guild's members.
        /// </summary>
        private async Task<IResult> PutRevokeRoleToMemberList(ulong aRoleId, string[] aFullDiscordHandleList, IMandrilRolesService aMandrilRolesService, CancellationToken aCancellationToken = default)
            => await aMandrilRolesService.RevokeRoleToMemberList(aRoleId, aFullDiscordHandleList, aCancellationToken: aCancellationToken)
            .ToIResult();

        #endregion

    }
}
