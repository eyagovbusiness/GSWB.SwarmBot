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
            aWebApplication.MapGet("/getMemberHighestRole", GetMemberHighestRole).SetResponseMetadata<DiscordRoleDTO>(200, 404);

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
        /// Get the highest role in the server's role hierarchy assigned to the specified member by its User Id.
        /// </summary>
        private async Task<IResult> GetMemberHighestRole(string aUserId, IMandrilMembersService aMandrilMembersService, CancellationToken aCancellationToken = default)
            => await aMandrilMembersService.GetMemberHighestRole(Convert.ToUInt64(aUserId), aCancellationToken)
            .ToIResult();

        #endregion

    }
}
