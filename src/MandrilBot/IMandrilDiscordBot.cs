using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TGF.Common.ROP.HttpResult;

namespace MandrilBot
{
    /// <summary>
    /// Public interfacewith the accessible methods of the MandrilDiscordBot service, providing interaction with the internal bot.
    /// </summary>
    public interface IMandrilDiscordBot
    {
        /// <summary>
        /// Gets a HealthCheck information about this service by attempting to fetch the target discord guild through the bot client.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>
        /// <see cref="HealthCheckResult"/> healthy if the bot is up and working under 150ms latency, 
        /// dergraded in case latency is over 150ms and unhealthy in case the bot is down. </returns>
        Task<HealthCheckResult> GetHealthCheck(CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot add a given list of users to a given category channel and all inner channels. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aCategoryId">Id of the category channel</param>
        /// /// <param name="aUserFullHandleList">List of discord full handles</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        Task<IHttpResult<Unit>> AddMemberListToChannel(ulong aChannelId, string[] aUserFullHandleList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to a given user server in this context.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aRoleId">Id of the role to assign in this server to the user.</param>
        /// <param name="aFullDiscordHandle">string representing the full discord Handle with format {Username}#{Discriminator} of the user.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        Task<IHttpResult<Unit>> AssignRoleToMember(ulong aRoleId, string aFullDiscordHandle, string aReason = null, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every user in the given list from the server in this context.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aRoleId">Id of the role to assign in this server to the users.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {Username}#{Discriminator} of the users.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to create a new category in the context server from a given template. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aEventCategoryChannelTemplate"><see cref="EventCategoryChannelTemplate"/> template to follow on creating the new category.</param>
        /// <returns><see cref="IHttpResult{string}"/> with information about success or fail on this operation.</returns>
        Task<IHttpResult<string>> CreateCategoryFromTemplate(CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Gets a valid Id from <see cref="DiscordChannel"/> that is category if exist.
        /// </summary>
        /// <param name="aDiscordCategoryName"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="IHttpResult{string}"/> with valid DiscordChannel Id or default ulong value.</returns>
        Task<IHttpResult<string>> GetExistingCategoryId(string aDiscordCategoryName, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Synchronizes an existing <see cref="DiscordChannel"/> with the given <see cref="CategoryChannelTemplate"/> template, removing not matching channels and adding missing ones.
        /// </summary>
        /// <param name="aDiscordCategoryId"></param>
        /// <param name="aCategoryChannelTemplate"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="IHttpResult{Unit}"/> informing about success or failure in operation.</returns>
        Task<IHttpResult<Unit>> SyncExistingCategoryWithTemplate(ulong aDiscordCategoryId, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to create a new Role in the context server.
        /// </summary>
        /// <param name="aRoleName">string that will name the new Role.</param>
        /// <returns><see cref="IHttpResult{string}"/> with information about success or fail on this operation and the Id of the new Role if succeed.</returns>
        Task<IHttpResult<string>> CreateRole(string aRoleName, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to delete a given Role in the context server.
        /// </summary>
        /// <param name="aRoleId">string that represents the name the Role to delete.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        Task<IHttpResult<Unit>> DeleteRole(ulong aRoleId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot delete a given category channel and all inner channels. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aEventCategorylId">Id of the category channel</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        Task<IHttpResult<Unit>> DeleteCategoryFromId(ulong aEventCategorylId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to get if exist a user account with the given Id.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns><see cref="IHttpResult{bool}"/> with true if an user was found with the given Id, false otherwise</returns>
        Task<IHttpResult<bool>> ExistUser(ulong aUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to get if a given user has verified account. 
        /// </summary>
        /// <returns><see cref="IHttpResult{int}"/> with the number of online members.</returns>
        Task<IHttpResult<int>> GetNumberOfOnlineMembers(CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to get the date of creation of the given user account.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns><see cref="IHttpResult{DateTimeOffset}"/> with the date of creation of the given user account.</returns>
        Task<IHttpResult<DateTimeOffset>> GetUserCreationDate(ulong aUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to get if a given user has verified account. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aFullDiscordHandle">string representing the full discord Handle with format {Username}#{Discriminator} of the user.</param>
        /// <returns><see cref="IHttpResult{bool}"/> with true if the user has verified account, false otherwise</returns>
        Task<IHttpResult<bool>> IsUserVerified(ulong aUserId, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every user in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the users.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {Username}#{Discriminator} of the users.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Attempts asynchronously to establish the connection of the internal configured bot with Discord.
        /// </summary>
        /// <returns>awaitable <see cref="Task"/></returns>
        public Task StartAsync();
    }
}
