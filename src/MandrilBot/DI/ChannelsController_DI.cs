using DSharpPlus;
using DSharpPlus.Entities;
using MandrilBot.Handelers;
using MandrilBot.Handlers;
using MediatR;
using TGF.Common.Extensions;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilBot.Controllers
{
    /// <summary>
    /// Class that contains the logic to perform all the public operations related with Discord channels.
    /// </summary>
    public partial class ChannelsController : IChannelsController
    {
        private readonly GuildsHandler _guildsHandler;
        public ChannelsController(IMandrilDiscordBot aMandrilDiscordBot) 
            => _guildsHandler = new GuildsHandler(aMandrilDiscordBot);

    }

    /// <summary>
    /// Public interface of Channels controller that gives access to all the public operations related with Discord channels.
    /// </summary>
    public interface IChannelsController
    {
        /// <summary>
        /// Commands the discord bot to create a new category in the context server from the provided template. 
        /// </summary>
        /// <param name="aCategoryChannelTemplate"><see cref="CategoryChannelTemplate"/> template to follow on creating the new category.</param>
        /// <returns><see cref="IHttpResult{string}"/> with the Id of the created category channel and information about success or failureure on this operation.</returns>
        public Task<IHttpResult<string>> CreateCategoryFromTemplate(CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Gets a valid Id from <see cref="DiscordChannel"/> that is a Category channel if exist.
        /// </summary>
        /// <param name="aDiscordCategoryName">Name of the category from which to get the Id</param>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="IHttpResult{string}"/> with valid DiscordChannel Id and information about success or failureure on this operation.</returns>
        public Task<IHttpResult<string>> GetExistingCategoryId(string aDiscordCategoryName, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Synchronizes an existing <see cref="DiscordChannel"/> with the given <see cref="CategoryChannelTemplate"/> template, removing not matching channels and adding missing ones.
        /// </summary>
        /// <param name="aDiscordCategoryId"></param>
        /// <param name="aCategoryChannelTemplate"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns>awaitable <see cref="Task"/> with <see cref="IHttpResult{Unit}"/> informing about success or failureure in operation.</returns>
        public Task<IHttpResult<Unit>> SyncExistingCategoryWithTemplate(ulong aDiscordCategoryId, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot delete a given category channel and all inner channels. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aEventCategorylId">Id of the category channel</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or failure on this operation.</returns>
        public Task<IHttpResult<Unit>> DeleteCategoryFromId(ulong aEventCategorylId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot add a given list of users to a given category channel and all inner channels. 
        /// </summary>
        /// /// <param name="aUserFullHandleList">List of discord full handles</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or failure on this operation.</returns>
        public Task<IHttpResult<Unit>> AddMemberListToChannel(ulong aChannelId, string[] aUserFullHandleList, CancellationToken aCancellationToken = default);

    }
}
