using Mandril.Application;
using Mandril.Application.DTOs;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordMemberEndpointDefinitions : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet("/getNumberOfOnlineMembers", GetNumberOfOnlineMembers).SetResponseMetadata<int>(200);
            aWebApplication.MapGet("/getMemberNicknameFromId", GetMemberNicknameFromId).SetResponseMetadata<string>(200, 404);
            aWebApplication.MapGet("/getMemberRoleList", GetMemberRoleList).SetResponseMetadata<IEnumerable<DiscordRoleDTO>>(200, 404);

        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        #endregion

        #region EndpointMethods

        /// <summary>
        /// Get the number of guild members online.
        /// </summary>
        private async Task<IResult> GetNumberOfOnlineMembers(IMandrilMembersService aMandrilMembersService, CancellationToken aCancellationToken = default)
            => await aMandrilMembersService.GetNumberOfOnlineMembers(aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Get the member's server nickname from the Discord user id.
        /// </summary>
        private async Task<IResult> GetMemberNicknameFromId(string aDiscordUserId, IMandrilMembersService aMandrilMembersService, CancellationToken aCancellationToken = default)
            => await aMandrilMembersService.GetMemberNicknameFromUserId(aDiscordUserId, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Get the the list of all assigned roles to this member in the guild's server ordered by position.
        /// </summary>
        private async Task<IResult> GetMemberRoleList(string aUserId, IMandrilMembersService aMandrilMembersService, CancellationToken aCancellationToken = default)
            => await aMandrilMembersService.GetMemberRoleList(Convert.ToUInt64(aUserId), aCancellationToken)
            .ToIResult();

        #endregion

    }
}
