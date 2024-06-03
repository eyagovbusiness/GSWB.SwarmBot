using SwarmBot.Application;
using Common.Application.DTOs.Discord;
using Microsoft.AspNetCore.Mvc;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;
using TGF.Common.ROP.HttpResult;
using Common.Infrastructure.Communication.ApiRoutes;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordChannelEndpoints : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(SwarmBotApiRoutes.channels_categories_byName, GetExistingCategoryId).SetResponseMetadata<ulong>(200, 404);
            aWebApplication.MapPost(SwarmBotApiRoutes.channels_categories, PostCreateCategoryFromTemplate).SetResponseMetadata<ulong>(200, 404);
            aWebApplication.MapPut(SwarmBotApiRoutes.channels_categories_members, PutAddMemberListToCategory).SetResponseMetadata(200, 404);
            aWebApplication.MapPut(SwarmBotApiRoutes.channels_categories, PutUpdateCategoryFromTemplate).SetResponseMetadata(200, 404);
            aWebApplication.MapDelete(SwarmBotApiRoutes.channels_categories, DeleteCategory).SetResponseMetadata(200, 404);

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
        private async Task<IResult> GetExistingCategoryId(string name, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken aCancellationToken = default)
            => await aSwarmBotChannelsService.GetExistingCategoryId(name, aCancellationToken)
            .Map(categoryId => categoryId.ToString())
            .ToIResult();

        /// <summary>
        /// Create a new category in the context server from the provided template. 
        /// </summary>
        private async Task<IResult> PostCreateCategoryFromTemplate([FromBody] CategoryChannelTemplateDTO aTemplate, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken aCancellationToken = default)
            => await aSwarmBotChannelsService.CreateCategoryFromTemplate(aTemplate, aCancellationToken)
            .Map(newCategoryId => newCategoryId.ToString())
            .ToIResult();

        /// <summary>
        /// Add a given list of members to a given category channel and all inner channels. 
        /// </summary>
        private async Task<IResult> PutAddMemberListToCategory(ulong id, string[] aUserHandleList, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken aCancellationToken = default)
            => await aSwarmBotChannelsService.AddMemberListToChannel(id, aUserHandleList, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Synchronizes an existing discord category channel with the given category template, removing not matching channels and adding missing ones.
        /// </summary>
        private async Task<IResult> PutUpdateCategoryFromTemplate(ulong id, CategoryChannelTemplateDTO aTemplate, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken aCancellationToken = default)
            => await aSwarmBotChannelsService.SyncExistingCategoryWithTemplate(id, aTemplate, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Delete a given category channel and all inner channels. 
        /// </summary>
        private async Task<IResult> DeleteCategory(ulong id, ISwarmBotChannelsService aSwarmBotChannelsService, CancellationToken aCancellationToken = default)
            => await aSwarmBotChannelsService.DeleteCategoryFromId(id, aCancellationToken)
            .ToIResult();

        #endregion

    }
}
