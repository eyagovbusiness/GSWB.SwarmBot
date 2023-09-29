using Mandril.Application;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordUserEndpoints : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(MandrilApiRoutes.users_exist, Get_UserExist).SetResponseMetadata<bool>(200, 404);
            aWebApplication.MapGet(MandrilApiRoutes.users_isVerified, Get_UserIsVerified).SetResponseMetadata<bool>(200, 404);
            aWebApplication.MapGet(MandrilApiRoutes.users_creationDate, Get_UserCreationDate).SetResponseMetadata<DateTimeOffset>(200, 404);

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
        private async Task<IResult> Get_UserExist(ulong userId, IMandrilUsersService aMandrilUsersService, CancellationToken aCancellationToken = default)
            => await aMandrilUsersService.ExistUser(userId, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Gets if a given Discord user under a given Id is verified.
        /// </summary>
        private async Task<IResult> Get_UserIsVerified(ulong userId, IMandrilUsersService aMandrilUsersService, CancellationToken aCancellationToken = default)
            => await aMandrilUsersService.IsUserVerified(userId, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Gets the creation time of a given Discord user under a given Id is verified.
        /// </summary>
        private async Task<IResult> Get_UserCreationDate(ulong userId, IMandrilUsersService aMandrilUsersService, CancellationToken aCancellationToken = default)
            => await aMandrilUsersService.GetUserCreationDate(userId, aCancellationToken)
            .ToIResult();

        #endregion

    }
}
