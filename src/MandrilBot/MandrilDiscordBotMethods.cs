using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using TGF.Common.Extensions;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilBot
{
    public partial class MandrilDiscordBot : IMandrilDiscordBot
    {
        internal static readonly byte _maxDegreeOfParallelism = Convert.ToByte(Math.Ceiling(Environment.ProcessorCount * 0.75));

        /// <summary>
        /// Gets a HealthCheck information about this service by attempting to fetch the target discord guild through the bot client.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns>
        /// <see cref="HealthCheckResult"/> healthy if the bot is up and working under 150ms latency, 
        /// dergraded in case latency is over 150ms and unhealthy in case the bot is down. </returns>
        public async Task<HealthCheckResult> GetHealthCheck(CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            try
            {
                var lGuilPreview = await Client.GetGuildPreviewAsync(_botConfiguration.DiscordTargetGuildId);
                if (lGuilPreview?.ApproximateMemberCount != null && lGuilPreview?.ApproximateMemberCount > 0)
                {
                    if (Client.Ping < 150)
                        return HealthCheckResult.Healthy(string.Format("MandrilDiscordBot service is healthy. ({0}ms) ping", Client.Ping));
                    else
                        return HealthCheckResult.Degraded(string.Format("MandrilDiscordBot service is degraded due to high latency. ({0}ms) ping", Client.Ping));
                }
            }
            catch (Exception lException)
            {
                return HealthCheckResult.Unhealthy("MandrilDiscordBot service is down at the moment, an exception was thrown fetching the guild preview.", lException);
            }
            return HealthCheckResult.Unhealthy("MandrilDiscordBot service is down at th moment, could not fetch guild preview.");

        }

        #region UserVerify

        /// <summary>
        /// Commands this discord bot to get if exist a user account with the given Id.
        /// </summary>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns><see cref="IHttpResult{bool}"/> with true if an user was found with the given Id, false otherwise</returns>
        public async Task<IHttpResult<bool>> ExistUser(ulong aUserId, CancellationToken aCancellationToken = default)
            => await GetUserAsync(aUserId, aCancellationToken)
                    .Map(discordUser => discordUser != null);

        /// <summary>
        /// Commands this discord bot to get if a given user has verified account. 
        /// </summary>
        /// <param name="aFullDiscordHandle">string representing the full discord Handle with format {Username}#{Discriminator} of the user.</param>
        /// <returns><see cref="IHttpResult{bool}"/> with true if the user has verified account, false otherwise</returns>
        public async Task<IHttpResult<bool>> IsUserVerified(ulong aUserId, CancellationToken aCancellationToken = default)//TO-DO: WRONG ERROR WHEN INVALID ID
            => await GetUserAsync(aUserId, aCancellationToken)
                    .Map(discordUser => discordUser.Verified.GetValueOrDefault(false));

        /// <summary>
        /// Commands this discord bot to get the date of creation of the given user account.
        /// </summary>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns><see cref="IHttpResult{DateTimeOffset}"/> with the date of creation of the given user account.</returns>
        public async Task<IHttpResult<DateTimeOffset>> GetUserCreationDate(ulong aUserId, CancellationToken aCancellationToken = default)//TO-DO: WRONG ERROR WHEN INVALID ID
            => await GetUserAsync(aUserId, aCancellationToken)
                    .Map(discordUser => discordUser.CreationTimestamp);

        #endregion

        #region Roles

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to a given user server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the user.</param>
        /// <param name="aFullDiscordHandle">string representing the full discord Handle with format {Username}#{Discriminator} of the user.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> AssignRoleToMember(ulong aRoleId, string aFullDiscordHandle, string aReason = null, CancellationToken aCancellationToken = default)
        {
            DiscordGuild lDiscordGuild = default; DiscordRole lDiscordRole = default;
            return await ValidateMemberHandle(aFullDiscordHandle)
                        .Bind(_ => GetDiscordGuildFromConfigAsync(aCancellationToken))
                        .Tap(discordGuild => lDiscordGuild = discordGuild)
                        .Bind(discordGuild => GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken))
                        .Tap(discordRole => lDiscordRole = discordRole)
                        .Bind(discordGuild => GetDiscordMemberAtmAsync(lDiscordGuild, aFullDiscordHandle, aCancellationToken))
                        .Bind(discordMember => GrantRoleToMemberAtmAsync(discordMember, lDiscordRole, aReason, aCancellationToken));

        }

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every user in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the users.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {Username}#{Discriminator} of the users.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> AssignRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default;
            return await ValidateMemberHandleList(aFullHandleList)
                        .Bind(_ => GetDiscordGuildFromConfigAsync(aCancellationToken))
                        .Bind(discordGuild => GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                        .Tap(discordRole=> lDiscordRole = discordRole)
                                             .Bind(_ => GetDiscordMemberListAtmAsync(discordGuild, aFullHandleList, aCancellationToken))
                                             .Bind(discordMemberList => GrantRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));

        }

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every user in the given list from the server in this context.
        /// </summary>
        /// <param name="aRoleId">Id of the role to assign in this server to the users.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {Username}#{Discriminator} of the users.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> RevokeRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordRole = default;
            return await ValidateMemberHandleList(aFullHandleList)
                        .Bind(_ => GetDiscordGuildFromConfigAsync(aCancellationToken))
                        .Bind(discordGuild => GetDiscordRoleAtm(discordGuild, aRoleId, aCancellationToken)
                        .Tap(discordRole => lDiscordRole = discordRole)
                                             .Bind(_ => GetDiscordMemberListAtmAsync(discordGuild, aFullHandleList, aCancellationToken))
                                             .Bind(discordMemberList => RevokeRoleToMemberListAtmAsync(discordMemberList, lDiscordRole)));

        }

        /// <summary>
        /// Commands this discord bot to create a new Role in the context server.
        /// </summary>
        /// <param name="aRoleName">string that will name the new Role.</param>
        /// <returns><see cref="IHttpResult{string}"/> with information about success or fail on this operation and the Id of the new Role if succeed.</returns>
        public async Task<IHttpResult<string>> CreateRole(string aRoleName, CancellationToken aCancellationToken = default)
            => await GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => CreateRoleAtmAsync(discordGuild, aRoleName, aCancellationToken));

        /// <summary>
        /// Commands this discord bot to delete a given Role in the context server.
        /// </summary>
        /// <param name="aRoleId">string that represents the name the Role to delete.</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> DeleteRole(ulong aRoleId, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken).Bind(_=>GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => DeleteRoleAtmAsync(discordGuild, aRoleId, aCancellationToken)));

        #endregion

        #region Guild

        /// <summary>
        /// Gets the number of total members connected at this moment in the guild server.
        /// </summary>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="IHttpResult{int}"/> with the number of connected members.</returns>
        public async Task<IHttpResult<int>> GetNumberOfOnlineMembers(CancellationToken aCancellationToken = default)
            => await GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => GetAllDiscordMemberListAtmAsync(discordGuild, aCancellationToken))
                    .Map(discordMemberList => discordMemberList.Count(x => x.Presence != null
                                                                            && x.Presence.Status == UserStatus.Online
                                                                            && x.VoiceState?.Channel != null));

        /// <summary>
        /// Commands this discord bot to create a new category in the context server from a given template. 
        /// </summary>
        /// <param name="aEventCategoryChannelTemplate"><see cref="EventCategoryChannelTemplate"/> template to follow on creating the new category.</param>
        /// <returns><see cref="IHttpResult{string}"/> with the Id of the created category channel and information about success or fail on this operation.</returns>
        public async Task<IHttpResult<string>> CreateCategoryFromTemplate(CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
        {
            DiscordRole lDiscordEveryoneRole = default;
            return await GetDiscordGuildFromConfigAsync(aCancellationToken)
                        .Bind(discordGuild => GetDiscordRoleAtm(discordGuild, discordGuild.Id, aCancellationToken)
                        .Bind(_ => CreateTemplateChannelsAtmAsync(discordGuild, lDiscordEveryoneRole, aCategoryChannelTemplate, aCancellationToken)));

        }

        /// <summary>
        /// Gets a valid Id from <see cref="DiscordChannel"/> that is category if exist.
        /// </summary>
        /// <param name="aDiscordCategoryName"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="IHttpResult{string}"/> with valid DiscordChannel Id or default ulong value.</returns>
        public async Task<IHttpResult<string>> GetExistingCategoryId(string aDiscordCategoryName, CancellationToken aCancellationToken = default)
            => await GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => GetCategoryIdFromName(discordGuild, aDiscordCategoryName, aCancellationToken))
                    .Map(discordChannel => discordChannel.Id.ToString());

        ///// <summary>
        ///// Synchronizes an existing <see cref="DiscordChannel"/> with the given <see cref="CategoryChannelTemplate"/> template, removing not matching channels and adding missing ones.
        ///// </summary>
        ///// <param name="aDiscordCategoryId"></param>
        ///// <param name="aCategoryChannelTemplate"></param>
        ///// <param name="aCancellationToken"></param>
        ///// <returns>awaitable <see cref="Task"/> with <see cref="IHttpResult{T}"/> informing about success or failure in operation.</returns>
        //public async Task<Result> SyncExistingCategoryWithTemplate(ulong aDiscordCategoryId, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
        //{
        //    var lGuildRes = await this.TryGetDiscordGuildFromConfigAsync(aCancellationToken);
        //    if (!lGuildRes.IsSuccess)
        //        return Result.Failure(lGuildRes.Error);

        //    var lExistingCategory = lGuildRes.Value.GetChannel(aDiscordCategoryId);
        //    if (lExistingCategory == null)
        //        return Result.Failure(DiscordBotErrors.Channel.NotFoundId);

        //    await lExistingCategory.SyncExistingCategoryWithTemplate_Delete(aCategoryChannelTemplate, aCancellationToken);
        //    await lExistingCategory.SyncExistingCategoryWithTemplate_Create(aCategoryChannelTemplate, aCancellationToken);

        //    return Result.Success();

        //}

        ///// <summary>
        ///// Commands this discord bot delete a given category channel and all inner channels. 
        ///// </summary>
        ///// <param name="aBot">Current discord bot that will execute the commands.</param>
        ///// <param name="aEventCategorylId">Id of the category channel</param>
        ///// <returns><see cref="IHttpResult{T}"/> with information about success or fail on this operation.</returns>
        //public async Task<Result> DeleteCategoryFromId(ulong aEventCategorylId, CancellationToken aCancellationToken = default)
        //{
        //    var lGuildRes = await this.TryGetDiscordGuildFromConfigAsync(aCancellationToken);
        //    if (!lGuildRes.IsSuccess)
        //        return Result.Failure(lGuildRes.Error);

        //    var lChannelRes = await this.TryGetDiscordChannelAsync(aEventCategorylId, aCancellationToken);
        //    if (!lChannelRes.IsSuccess)
        //        return Result.Failure(lChannelRes.Error);

        //    await lChannelRes.Value.Children.ParallelForEachAsync(
        //        _maxDegreeOfParallelism,
        //        x => x.DeleteAsync("Event finished"),
        //        aCancellationToken);
        //    await lChannelRes.Value.DeleteAsync("Event finished");
        //    return Result.Success();

        //}

        ///// <summary>
        ///// Commands this discord bot add a given list of users to a given category channel and all inner channels. 
        ///// </summary>
        ///// /// <param name="aUserFullHandleList">List of discord full handles</param>
        ///// <returns><see cref="IHttpResult{T}"/> with information about success or fail on this operation.</returns>
        //public async Task<Result> AddMemberListToChannel(ulong aChannelId, string[] aUserFullHandleList, CancellationToken aCancellationToken = default)
        //{
        //    var lGuildRes = await this.TryGetDiscordGuildFromConfigAsync(aCancellationToken);
        //    if (!lGuildRes.IsSuccess)
        //        return Result.Failure(lGuildRes.Error);

        //    var lChannelRes = await this.TryGetDiscordChannelAsync(aChannelId, aCancellationToken);
        //    if (!lChannelRes.IsSuccess)
        //        return Result.Failure(lChannelRes.Error);

        //    var lMemberListRes = await lGuildRes.Value.TryGetGuildMemberListFromHandlesAsync(aUserFullHandleList, aCancellationToken);
        //    if (!lMemberListRes.IsSuccess)
        //        return Result.Failure(lMemberListRes.Error);

        //    var lOverWriteBuilderList = lMemberListRes.Value
        //        .Select(x => new DiscordOverwriteBuilder(x).Allow(Permissions.AccessChannels | Permissions.UseVoice))
        //        .ToList();//Materialize list to avoid double materializing below 

        //    //As far as lChannelRes.Value.PermissionOverwrites is write-only, we have to copy current channel's overwrites and add them to the new ones we want to add.
        //    await lChannelRes.Value.PermissionOverwrites.ParallelForEachAsync(
        //        _maxDegreeOfParallelism,
        //        x => lOverWriteBuilderList.UpdateBuilderOverwrites(x),
        //        aCancellationToken);

        //    //We override the overrites with our new ones plus the already existing ones as we aded them to lOverWriteBuilderList below.
        //    await lChannelRes.Value.ModifyAsync(x => x.PermissionOverwrites = lOverWriteBuilderList);
        //    await lChannelRes.Value.Children.ParallelForEachAsync(
        //        _maxDegreeOfParallelism,
        //        x => x.ModifyAsync(x => x.PermissionOverwrites = lOverWriteBuilderList),
        //        aCancellationToken);
        //    return Result.Success();

        //}

        #endregion

    }
}
