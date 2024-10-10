using Common.Infrastructure.Communication.ApiRoutes;
using SwarmBot.Application;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class TesterEndpoints : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(SwarmBotApiRoutes.private_guilds_testers, GetIsTester).SetResponseMetadata<Unit>(200, 404);
        }
        
        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        #endregion

        #region EndpointMethods

        /// <summary>
        /// Get from a Discord User ID the associated DiscordMember who is a Tester or 404 if the provided discord user id is not a tester.
        /// A discord user ID is tester if it is a member of the testers guild and it has the "Tester" role assigned.
        /// </summary>
        private async Task<IResult> GetIsTester(string guildId, string userId, ISwarmBotMembersService aSwarmBotMembersService, CancellationToken aCancellationToken = default)
            => await aSwarmBotMembersService.GetTester(ulong.Parse(guildId), ulong.Parse(userId), aCancellationToken)
            .Map(tester => Unit.Value)
            .ToIResult();

        #endregion

    }
}
