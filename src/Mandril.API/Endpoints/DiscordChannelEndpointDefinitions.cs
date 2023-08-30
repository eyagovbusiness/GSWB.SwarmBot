using TGF.CA.Presentation;
using TGF.CA.Presentation.MinimalAPI;
using TGF.CA.Presentation.Middleware;
using Mandril.Application;
using Microsoft.AspNetCore.Mvc;
using Mandril.Application.DTOs;

namespace Maindril.API.Endpoints
{
    /// <inheritdoc/>
    public class DiscordChannelEndpointDefinitions : IEndpointDefinition
    {

        #region IEndpointDefinition

        /// <inheritdoc/>
        public void DefineEndpoints(WebApplication aWebApplication)
        {
            aWebApplication.MapGet("/getExistingCategoryId", GetExistingCategoryId).SetResponseMetadata<string>(200, 404);
            aWebApplication.MapPost("/createCategoryFromTemplate", PostCreateCategoryFromTemplate).SetResponseMetadata<string>(200, 404);
            aWebApplication.MapPut("/addMemberListToCategory", PutAddMemberListToCategory).SetResponseMetadata(200, 404);
            aWebApplication.MapPut("/updateCategoryFromTemplate", PutUpdateCategoryFromTemplate).SetResponseMetadata(200, 404);
            aWebApplication.MapDelete("/deleteCategory", DeleteCategory).SetResponseMetadata(200, 404);

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
        private async Task<IResult> GetExistingCategoryId(string aCategoryName, IMandrilChannelsService aMandrilChannelsService, CancellationToken aCancellationToken = default)
            => await aMandrilChannelsService.GetExistingCategoryId(aCategoryName, aCancellationToken)
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
        private async Task<IResult> PutAddMemberListToCategory(ulong aCategoryId, string[] aUserFullHandleList, IMandrilChannelsService aMandrilChannelsService, CancellationToken aCancellationToken = default)
            => await aMandrilChannelsService.AddMemberListToChannel(aCategoryId, aUserFullHandleList, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Synchronizes an existing discord category channel with the given category template, removing not matching channels and adding missing ones.
        /// </summary>
        private async Task<IResult> PutUpdateCategoryFromTemplate(ulong aCategoryId, CategoryChannelTemplateDTO aTemplate, IMandrilChannelsService aMandrilChannelsService, CancellationToken aCancellationToken = default)
            => await aMandrilChannelsService.SyncExistingCategoryWithTemplate(aCategoryId, aTemplate, aCancellationToken)
            .ToIResult();

        /// <summary>
        /// Delete a given category channel and all inner channels. 
        /// </summary>
        private async Task<IResult> DeleteCategory(ulong aCategoryId, IMandrilChannelsService aMandrilChannelsService, CancellationToken aCancellationToken = default)
            => await aMandrilChannelsService.DeleteCategoryFromId(aCategoryId, aCancellationToken)
            .ToIResult();

        #endregion

    }
}
