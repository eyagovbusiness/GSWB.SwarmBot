using Mandril.Application;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordUserEndpointDefinitions : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet("/getUserExist", GetUserExist).SetResponseMetadata<bool>(200, 404);
            aWebApplication.MapGet("/getUserIsVerified", GetUserIsVerified).SetResponseMetadata<bool>(200, 404);
            aWebApplication.MapGet("/getUserCreationDate", GetUserCreationDate).SetResponseMetadata<DateTimeOffset>(200, 404);

        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        #endregion

        #region EndpointMethods

        /// <summary>
        /// Gets if a given Discord user exists under a given Id.
        /// </summary>
        private async Task<IResult> GetUserExist(ulong aUserId, IMandrilUsersService aMandrilUsersService, CancellationToken aCancellationToken = default)
            => await aMandrilUsersService.ExistUser(aUserId, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Gets if a given Discord user under a given Id is verified.
        /// </summary>
        private async Task<IResult> GetUserIsVerified(ulong aUserId, IMandrilUsersService aMandrilUsersService, CancellationToken aCancellationToken = default)
            => await aMandrilUsersService.IsUserVerified(aUserId, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Gets the creation time of a given Discord user under a given Id is verified.
        /// </summary>
        private async Task<IResult> GetUserCreationDate(ulong aUserId, IMandrilUsersService aMandrilUsersService, CancellationToken aCancellationToken = default)
            => await aMandrilUsersService.GetUserCreationDate(aUserId, aCancellationToken)
            .ToIResult();

        #endregion

    }
}
