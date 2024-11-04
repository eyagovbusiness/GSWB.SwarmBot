using SwarmBot.Application;
using Common.Application.DTOs.Discord;
using TGF.CA.Presentation;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.Result;
using Common.Domain.Validation;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using Common.Application.Communication.Routing;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordMemberEndpoints : IEndpointsDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(SwarmBotApiRoutes.private_guilds_members_countOnline, GetNumberOfOnlineMembers)
                .SetResponseMetadata<int>(200);

            aWebApplication.MapGet(SwarmBotApiRoutes.private_currentGuild_members_profile, GetMemberProfileFromId)
                .SetResponseMetadata<DiscordProfileDTO>(200, 404);

            aWebApplication.MapGet(SwarmBotApiRoutes.private_guilds_members_roles, GetMemberRoleList)
                .SetResponseMetadata<IEnumerable<DiscordRoleDTO>>(200, 404);

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
        private async Task<IResult> GetNumberOfOnlineMembers(string id, DiscordIdValidator discordIdValidator, ISwarmBotMembersService aSwarmBotMembersService, CancellationToken aCancellationToken = default)
         => await Result.ValidationResult(discordIdValidator.Validate(id))
        .Bind(_ => aSwarmBotMembersService.GetNumberOfOnlineMembers(ulong.Parse(id), aCancellationToken))
        .ToIResult();

        /// <summary>
        /// Get the member's server nickname from the Discord user id.
        /// </summary>
        private async Task<IResult> GetMemberProfileFromId(string guildId, ulong userId, ISwarmBotMembersService aSwarmBotMembersService, CancellationToken aCancellationToken = default)
        => await aSwarmBotMembersService.GetMemberProfileFromId(ulong.Parse(guildId), userId, aCancellationToken)
        .ToIResult();

        /// <summary>
        /// Get the the list of all assigned roles to this member in the guild's server ordered by position.
        /// </summary>
        private async Task<IResult> GetMemberRoleList(string guildId, string userId, DiscordIdValidator discordIdValidator, ISwarmBotMembersService aSwarmBotMembersService, CancellationToken aCancellationToken = default)
        => await Result.ValidationResult(discordIdValidator.Validate(guildId))
        .Validate(userId, discordIdValidator)
        .Bind(_ => aSwarmBotMembersService.GetMemberRoleList(Convert.ToUInt64(guildId), Convert.ToUInt64(userId), aCancellationToken))
        .ToIResult();

        #endregion

    }
}
