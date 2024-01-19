using DSharpPlus.Entities;
using SwarmBot.Application.DTOs;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;

namespace SwarmBot.Application
{
    /// <summary>
    /// Public interface of Channels controller that gives access to all the public operations related with Discord channels.
    /// </summary>
    public interface ISwarmBotChannelsService
    {
        /// <summary>
        /// Commands the discord bot to create a new category in the context server from the provided template. 
        /// </summary>
        /// <param name="aCategoryChannelTemplate"><see cref="CategoryChannelTemplateDTO"/> template to follow on creating the new category.</param>
        /// <returns><see cref="IHttpResult{ulong}"/> with the Id of the created category channel and information about success or failure on this operation.</returns>
        public Task<IHttpResult<ulong>> CreateCategoryFromTemplate(CategoryChannelTemplateDTO aCategoryChannelTemplate, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Gets the first <see cref="DiscordChannel"/> from the context server channel list that stratifies the given filter conditions.
        /// </summary>
        /// <param name="aFilterFunc">Filter function to get the required channel.</param>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="DiscordChannel"/></returns>
        public Task<IHttpResult<DiscordChannel>> GetDiscordChannel(Func<DiscordChannel, bool> aFilterFunc, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Gets a valid Id from <see cref="DiscordChannel"/> that is a Category channel if exist.
        /// </summary>
        /// <param name="aDiscordCategoryName">Name of the category from which to get the Id</param>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="IHttpResult{ulong}"/> with valid DiscordChannel Id and information about success or failure on this operation.</returns>
        public Task<IHttpResult<ulong>> GetExistingCategoryId(string aDiscordCategoryName, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Synchronizes an existing <see cref="DiscordChannel"/> with the given <see cref="CategoryChannelTemplateDTO"/> template, removing not matching channels and adding missing ones.
        /// </summary>
        /// <param name="aDiscordCategoryId"></param>
        /// <param name="aCategoryChannelTemplate"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns>awaitable <see cref="Task"/> with <see cref="IHttpResult{Unit}"/> informing about success or failure in operation.</returns>
        public Task<IHttpResult<Unit>> SyncExistingCategoryWithTemplate(ulong aDiscordCategoryId, CategoryChannelTemplateDTO aCategoryChannelTemplate, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot delete a given category channel and all inner channels. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aEventCategoryId">Id of the category channel</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or failure on this operation.</returns>
        public Task<IHttpResult<Unit>> DeleteCategoryFromId(ulong aEventCategoryId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot add a given list of users to a given category channel and all inner channels. 
        /// </summary>
        /// /// <param name="aUserFullHandleList">List of discord full handles</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or failure on this operation.</returns>
        public Task<IHttpResult<Unit>> AddMemberListToChannel(ulong aChannelId, string[] aUserFullHandleList, CancellationToken aCancellationToken = default);

    }
}
