using SwarmBot.Application;
using SwarmBot.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.HttpResult;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordChannelEndpoints : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(SwarmBotApiRoutes.channels_categories_getId, GetExistingCategoryId).SetResponseMetadata<ulong>(200, 404);
            aWebApplication.MapPost(SwarmBotApiRoutes.channels_categories_create, PostCreateCategoryFromTemplate).SetResponseMetadata<ulong>(200, 404);
            aWebApplication.MapPut(SwarmBotApiRoutes.channels_categories_addMemberList, PutAddMemberListToCategory).SetResponseMetadata(200, 404);
            aWebApplication.MapPut(SwarmBotApiRoutes.channels_categories_update, PutUpdateCategoryFromTemplate).SetResponseMetadata(200, 404);
            aWebApplication.MapDelete(SwarmBotApiRoutes.channels_categories_delete, DeleteCategory).SetResponseMetadata(200, 404);

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
        private async Task<IResult> GetExistingCategoryId(string categoryName, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken? aCancellationToken = default)
            => await aSwarmBotChannelsService.GetExistingCategoryId(categoryName, aCancellationToken.GetValueOrDefault())
            .Map(categoryId => categoryId.ToString())
            .ToIResult();

        /// <summary>
        /// Create a new category in the context server from the provided template. 
        /// </summary>
        private async Task<IResult> PostCreateCategoryFromTemplate([FromBody] CategoryChannelTemplateDTO aTemplate, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken? aCancellationToken = default)
            => await aSwarmBotChannelsService.CreateCategoryFromTemplate(aTemplate, aCancellationToken.GetValueOrDefault())
            .Map(newCategoryId => newCategoryId.ToString())
            .ToIResult();

        /// <summary>
        /// Add a given list of members to a given category channel and all inner channels. 
        /// </summary>
        private async Task<IResult> PutAddMemberListToCategory(ulong categoryId, string[] aUserHandleList, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken? aCancellationToken = default)
            => await aSwarmBotChannelsService.AddMemberListToChannel(categoryId, aUserHandleList, aCancellationToken.GetValueOrDefault())
            .ToIResult();

        /// <summary>
        /// Synchronizes an existing discord category channel with the given category template, removing not matching channels and adding missing ones.
        /// </summary>
        private async Task<IResult> PutUpdateCategoryFromTemplate(ulong categoryId, CategoryChannelTemplateDTO aTemplate, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken? aCancellationToken = default)
            => await aSwarmBotChannelsService.SyncExistingCategoryWithTemplate(categoryId, aTemplate, aCancellationToken.GetValueOrDefault())
            .ToIResult();

        /// <summary>
        /// Delete a given category channel and all inner channels. 
        /// </summary>
        private async Task<IResult> DeleteCategory(ulong categoryId, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken? aCancellationToken = default)
            => await aSwarmBotChannelsService.DeleteCategoryFromId(categoryId, aCancellationToken.GetValueOrDefault())
            .ToIResult();

        #endregion

    }
}
