using Mandril.Application;
using Mandril.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using TGF.CA.Presentation;
using TGF.CA.Presentation.Middleware;
using TGF.CA.Presentation.MinimalAPI;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordChannelEndpoints : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet(MandrilApiRoutes.channels_categories_getId, GetExistingCategoryId).SetResponseMetadata<string>(200, 404);
            aWebApplication.MapPost(MandrilApiRoutes.channels_categories_create, PostCreateCategoryFromTemplate).SetResponseMetadata<string>(200, 404);
            aWebApplication.MapPut(MandrilApiRoutes.channels_categories_addMemberList, PutAddMemberListToCategory).SetResponseMetadata(200, 404);
            aWebApplication.MapPut(MandrilApiRoutes.channels_categories_update, PutUpdateCategoryFromTemplate).SetResponseMetadata(200, 404);
            aWebApplication.MapDelete(MandrilApiRoutes.channels_categories_delete, DeleteCategory).SetResponseMetadata(200, 404);

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
        private async Task<IResult> GetExistingCategoryId(string categoryName, IMandrilChannelsService aMandrilChannelsService, CancellationToken aCancellationToken = default)
            => await aMandrilChannelsService.GetExistingCategoryId(categoryName, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Create a new category in the context server from the provided template. 
        /// </summary>
        private async Task<IResult> PostCreateCategoryFromTemplate([FromBody] CategoryChannelTemplateDTO aTemplate, IMandrilChannelsService aMandrilChannelsService, CancellationToken aCancellationToken = default)
            => await aMandrilChannelsService.CreateCategoryFromTemplate(aTemplate, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Add a given list of members to a given category channel and all inner channels. 
        /// </summary>
        private async Task<IResult> PutAddMemberListToCategory(ulong categoryId, string[] aUserHandleList, IMandrilChannelsService aMandrilChannelsService, CancellationToken aCancellationToken = default)
            => await aMandrilChannelsService.AddMemberListToChannel(categoryId, aUserHandleList, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Synchronizes an existing discord category channel with the given category template, removing not matching channels and adding missing ones.
        /// </summary>
        private async Task<IResult> PutUpdateCategoryFromTemplate(ulong categoryId, CategoryChannelTemplateDTO aTemplate, IMandrilChannelsService aMandrilChannelsService, CancellationToken aCancellationToken = default)
            => await aMandrilChannelsService.SyncExistingCategoryWithTemplate(categoryId, aTemplate, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Delete a given category channel and all inner channels. 
        /// </summary>
        private async Task<IResult> DeleteCategory(ulong categoryId, IMandrilChannelsService aMandrilChannelsService, CancellationToken aCancellationToken = default)
            => await aMandrilChannelsService.DeleteCategoryFromId(categoryId, aCancellationToken)
            .ToIResult();

        #endregion

    }
}
