using Common.Application.Communication.Routing;
using SwarmBot.Application;
using SwarmBot.Domain.ValueObjects;
using TGF.CA.Presentation;
using TGF.CA.Presentation.MinimalAPI;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class ScToolsEndpoints : IEndpointsDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(SwarmBotApiRoutes.scTools_listShips, GetListShips).SetResponseMetadata<Ship[]>(200);
        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        #endregion

        #region EndpointMethods

        /// <summary>
        /// Get all ship data from RSI web and available CCU in json format.
        /// </summary>
        private async Task<IResult> GetListShips(IScToolsService aScToolsService, CancellationToken aCancellationToken = default)
            => await aScToolsService.GetRsiShipList().ToIResult();

        #endregion

    }
}
