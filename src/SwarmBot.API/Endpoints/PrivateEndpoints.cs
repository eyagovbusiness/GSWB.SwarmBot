using Common.Domain.Validation;
using Common.Infrastructure.Communication.ApiRoutes;
using TGF.CA.Presentation;
using SwarmBot.Application;
using TGF.CA.Presentation.MinimalAPI;
using Common.Application.DTOs.Guilds;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Infrastructure.Security.Identity.Authentication;
using System.Security.Claims;

namespace SwarmBot.API.Endpoints
{
    /// List of private endpoint only reached from the internal private docker network.
    public class PrivateEndpoints : IEndpointDefinition
    {
        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(SwarmBotApiRoutes.private_users_me_guilds, Get_UserGuilds).RequireDiscord().SetResponseMetadata<GuildDTO[]>(200).ProducesValidationProblem();

        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        /// private endpoint implementation 
        /// <summary>
        /// Gets the list of guilds where both the SwarmBot and the user under the provided id are in.
        /// </summary>
        private async Task<IResult> Get_UserGuilds(ClaimsPrincipal claimsPrincipal, DiscordIdValidator discordIdValidator, ISwarmBotUsersService swarmBotUsersService, CancellationToken aCancellationToken = default)
        => await swarmBotUsersService.GetUserGuilds(ulong.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!), aCancellationToken)
        .ToIResult();

    }
}
