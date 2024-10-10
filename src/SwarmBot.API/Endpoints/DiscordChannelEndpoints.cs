using SwarmBot.Application;
using Common.Application.DTOs.Discord;
using Microsoft.AspNetCore.Mvc;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.HttpResult;
using Common.Infrastructure.Communication.ApiRoutes;
using System.Security.Claims;
using TGF.CA.Infrastructure.Security.Identity.Authentication;
using Common.Infrastructure.Security;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordChannelEndpoints : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(SwarmBotApiRoutes.channels_categories_byName, GetExistingCategoryId).RequireJWTBearer().SetResponseMetadata<ulong>(200, 404);
            aWebApplication.MapPost(SwarmBotApiRoutes.channels_categories, PostCreateCategoryFromTemplate).RequireJWTBearer().SetResponseMetadata<ulong>(200, 404);
            aWebApplication.MapPut(SwarmBotApiRoutes.channels_categories_members, PutAddMemberListToCategory).RequireJWTBearer().SetResponseMetadata(200, 404);
            aWebApplication.MapPut(SwarmBotApiRoutes.channels_categories, PutUpdateCategoryFromTemplate).RequireJWTBearer().SetResponseMetadata(200, 404);
            aWebApplication.MapDelete(SwarmBotApiRoutes.channels_categories, DeleteCategory).RequireJWTBearer().SetResponseMetadata(200, 404);

        }

        /// <inheritdoc/>
        public void DefineRequiredServices(IServiceCollection aRequiredServicesCollection)
        {
        }

        #endregion

        #region EndpointMethods

        /// <summary>
        /// Get a valid Id from a given discord channel by its name if it is a Category channel and exist.
        /// </summary>
        private async Task<IResult> GetExistingCategoryId(ClaimsPrincipal claimsPrincipal, string name, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken aCancellationToken = default)
            => await aSwarmBotChannelsService.GetExistingCategoryId(ulong.Parse(claimsPrincipal.FindFirstValue(GuildSwarmClaims.GuildId)!), name, aCancellationToken)
            .Map(categoryId => categoryId.ToString())
            .ToIResult();

        /// <summary>
        /// Create a new category in the context server from the provided template. 
        /// </summary>
        private async Task<IResult> PostCreateCategoryFromTemplate(ClaimsPrincipal claimsPrincipal, [FromBody] CategoryChannelTemplateDTO aTemplate, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken aCancellationToken = default)
            => await aSwarmBotChannelsService.CreateCategoryFromTemplate(ulong.Parse(claimsPrincipal.FindFirstValue(GuildSwarmClaims.GuildId)!), aTemplate, aCancellationToken)
            .Map(newCategoryId => newCategoryId.ToString())
            .ToIResult();

        /// <summary>
        /// Add a given list of members to a given category channel and all inner channels. 
        /// </summary>
        private async Task<IResult> PutAddMemberListToCategory(ClaimsPrincipal claimsPrincipal, ulong id, string[] aUserHandleList, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken aCancellationToken = default)
            => await aSwarmBotChannelsService.AddMemberListToChannel(ulong.Parse(claimsPrincipal.FindFirstValue(GuildSwarmClaims.GuildId)!), id, aUserHandleList, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Synchronizes an existing discord category channel with the given category template, removing not matching channels and adding missing ones.
        /// </summary>
        private async Task<IResult> PutUpdateCategoryFromTemplate(ClaimsPrincipal claimsPrincipal, ulong id, CategoryChannelTemplateDTO aTemplate, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken aCancellationToken = default)
            => await aSwarmBotChannelsService.SyncExistingCategoryWithTemplate(ulong.Parse(claimsPrincipal.FindFirstValue(GuildSwarmClaims.GuildId)!), id, aTemplate, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Delete a given category channel and all inner channels. 
        /// </summary>
        private async Task<IResult> DeleteCategory(ClaimsPrincipal claimsPrincipal, ulong id, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken aCancellationToken = default)
            => await aSwarmBotChannelsService.DeleteCategoryFromId(ulong.Parse(claimsPrincipal.FindFirstValue(GuildSwarmClaims.GuildId)!), id, aCancellationToken)
            .ToIResult();

        #endregion

    }
}
