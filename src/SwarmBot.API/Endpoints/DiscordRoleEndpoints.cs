using SwarmBot.Application;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.HttpResult;
using Common.Infrastructure.Communication.ApiRoutes;
using TGF.Common.ROP.Result;
using Common.Domain.Validation;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordRoleEndpoints : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapPost(SwarmBotApiRoutes.private_guilds_roles, Post_CreateRole)
                .SetResponseMetadata<ulong>(200, 404)
                .ProducesValidationProblem();

            aWebApplication.MapDelete(SwarmBotApiRoutes.private_guilds_role, Delete_Role)
                .SetResponseMetadata(200, 404)
                .ProducesValidationProblem();

            aWebApplication.MapPut(SwarmBotApiRoutes.private_roles_assign, Post_AssignRoleToMemberList)
                .SetResponseMetadata(200, 404)
                .ProducesValidationProblem();

            aWebApplication.MapPut(SwarmBotApiRoutes.private_roles_revoke, Put_RevokeRoleToMemberList)
                .SetResponseMetadata(200, 404)
                .ProducesValidationProblem();
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
        private async Task<IResult> Post_CreateRole(string guildId, string name, DiscordIdValidator discordIdValidator, ISwarmBotRolesService aSwarmBotRolesService, CancellationToken aCancellationToken = default)
        => await Result.ValidationResult(discordIdValidator.Validate(guildId))
        .Bind(_ => aSwarmBotRolesService.CreateRole(ulong.Parse(guildId), name, aCancellationToken))
        .Map(roleId => roleId.ToString())
        .ToIResult();

        /// <summary>
        /// Delete a given discord guild's role by its Id.
        /// </summary>
        private async Task<IResult> Delete_Role(string guildId, ulong roleId, DiscordIdValidator discordIdValidator, ISwarmBotRolesService aSwarmBotRolesService, CancellationToken aCancellationToken = default)
        => await Result.ValidationResult(discordIdValidator.Validate(guildId))
        .Bind(_ =>  aSwarmBotRolesService.DeleteRole(ulong.Parse(guildId), roleId, aCancellationToken))
        .ToIResult();

        /// <summary>
        /// Assign the given role by its Id to the given list of discord guild's members.
        /// </summary>
        private async Task<IResult> Post_AssignRoleToMemberList(string guildId, ulong roleId, string[] aDiscordHandleList, DiscordIdValidator discordIdValidator, ISwarmBotRolesService aSwarmBotRolesService, CancellationToken aCancellationToken = default)
        => await Result.ValidationResult(discordIdValidator.Validate(guildId))
        .Bind(_ => aSwarmBotRolesService.AssignRoleToMemberList(ulong.Parse(guildId), roleId, aDiscordHandleList, aCancellationToken: aCancellationToken))
        .ToIResult();

        /// <summary>
        /// Revoke the given role by its Id to the given list of discord guild's members.
        /// </summary>
        private async Task<IResult> Put_RevokeRoleToMemberList(string guildId, ulong roleId, string[] aDiscordHandleList, DiscordIdValidator discordIdValidator, ISwarmBotRolesService aSwarmBotRolesService, CancellationToken aCancellationToken = default)
        => await Result.ValidationResult(discordIdValidator.Validate(guildId))
        .Bind(_ => aSwarmBotRolesService.RevokeRoleToMemberList(ulong.Parse(guildId), roleId, aDiscordHandleList, aCancellationToken: aCancellationToken))
        .ToIResult();

        #endregion

    }
}
