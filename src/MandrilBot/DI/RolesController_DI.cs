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
    /// Class that contains the logic to perform all the public operations related with Discord Roles.
    /// </summary>
    public partial class RolesController : IRolesController
    {
        private readonly GuildsHandler _guildsHandler;
        public RolesController(IMandrilDiscordBot aMandrilDiscordBot)
            => _guildsHandler = new GuildsHandler(aMandrilDiscordBot);

    }

    /// <summary>
    /// Public interface of Roles controller that gives access to all the public operations related with Discord Roles.
    /// </summary>
    public interface IRolesController
    {
        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to a given user server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the user.</param>
        /// <param name="aFullDiscordHandle">string representing the full discord Handle with format {Username}#{Discriminator} of the user.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or failure on this operation.</returns>
        public Task<IHttpResult<Unit>> AssignRoleToMember(ulong aRoleId, string aFullDiscordHandle, string aReason = null, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every user in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the users.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {Username}#{Discriminator} of the users.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or failure on this operation.</returns>
        public Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every user in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the users.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {Username}#{Discriminator} of the users.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or failure on this operation.</returns>
        public Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to create a new Role in the context server.
        /// </summary>
        /// <param name="aRoleName">string that will name the new Role.</param>
        /// <returns><see cref="IHttpResult{string}"/> with information about success or failure on this operation and the Id of the new Role if succeed.</returns>
        public Task<IHttpResult<string>> CreateRole(string aRoleName, CancellationToken aCancellationToken = default);

        /// <summary>
        /// Commands this discord bot to delete a given Role in the context server.
        /// </summary>
        /// <param name="aRoleId">string that represents the name the Role to delete.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or failure on this operation.</returns>
        public Task<IHttpResult<Unit>> DeleteRole(ulong aRoleId, CancellationToken aCancellationToken = default);

    }
}
