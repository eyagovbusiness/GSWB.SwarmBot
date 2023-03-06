using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Cryptography;
using TGF.CA.Domain.Primitives;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.Extensions;

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
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns>True if an user was found with the given Id, false otherwise</returns>
        public async Task<Result<bool>> ExistUser(ulong aUserId, CancellationToken aCancellationToken = default)
        {
            return Result.Success(await this.GetUserAsync(aUserId, aCancellationToken) != null);
        }

        /// <summary>
        /// Commands this discord bot to get if a given user has verified account. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aFullDiscordHandle">string representing the full discord Handle with format {Username}#{Discriminator} of the user.</param>
        /// <returns>true if the user has verified account, false otherwise</returns>
        public async Task<Result<bool>> IsUserVerified(ulong aUserId, CancellationToken aCancellationToken = default)
        {
            var lUser = await this.GetUserAsync(aUserId, aCancellationToken);
            if (lUser == null)
                return Result.Failure<bool>(DiscordBotErrors.User.NotFoundId);
            return Result.Success(lUser.Verified.GetValueOrDefault(false));

        }

        /// <summary>
        /// Commands this discord bot to get the date of creation of the given user account.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns><see cref="DateTimeOffset"/> with the date of creation of the given user account.</returns>
        public async Task<Result<DateTimeOffset>> GetUserCreationDate(ulong aUserId, CancellationToken aCancellationToken = default)
        {
            var lUser = await this.GetUserAsync(aUserId, aCancellationToken);
            if (lUser == null)
                return Result.Failure<DateTimeOffset>(DiscordBotErrors.User.NotFoundId);
            return Result.Success(lUser.CreationTimestamp);

        }

        #endregion

        #region Roles

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to a given user server in this context.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aRoleId">Id of the role to assign in this server to the user.</param>
        /// <param name="aFullDiscordHandle">string representing the full discord Handle with format {Username}#{Discriminator} of the user.</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation.</returns>
        public async Task<Result> AssignRoleToMember(ulong aRoleId, string aFullDiscordHandle, string aReason = null, CancellationToken aCancellationToken = default)
        {
            var lRole = await this.TryGetDiscordRoleAsync(aRoleId, aCancellationToken);
            var lMember = await this.TryGetDiscordMemberAsync(aFullDiscordHandle, aCancellationToken);

            if (!lMember.IsSuccess)
                Result.Failure(DiscordBotErrors.Member.NotFoundHandle);

            await lMember.Value.GrantRoleAsync(lRole.Value, aReason, aCancellationToken);
            return Result.Success();

        }

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every user in the given list from the server in this context.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aRoleId">Id of the role to assign in this server to the users.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {Username}#{Discriminator} of the users.</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation.</returns>
        public async Task<Result> AssignRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default)
        {
            if (aFullHandleList.IsNullOrEmpty())
                return Result.Failure(DiscordBotErrors.List.Empty);

            var lTupleRes = await this.TryGetMemberListAndRoleAsync(aRoleId, aFullHandleList, aCancellationToken);
            if (!lTupleRes.IsSuccess)
                return Result.Failure(lTupleRes.Error);

            await lTupleRes.Value.Item1.ParallelForEachAsync(
                    _maxDegreeOfParallelism,
                    lMember => lMember.GrantRoleAsync(lTupleRes.Value.Item2),
                    aCancellationToken);

            return Result.Success();

        }

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every user in the given list from the server in this context.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aRoleId">Id of the role to assign in this server to the users.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {Username}#{Discriminator} of the users.</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation.</returns>
        public async Task<Result> RevokeRoleToMemberList(ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default)
        {
            if (aFullHandleList.IsNullOrEmpty())
                return Result.Failure(DiscordBotErrors.List.Empty);

            var lTupleRes = await this.TryGetMemberListAndRoleAsync(aRoleId, aFullHandleList, aCancellationToken);
            if (!lTupleRes.IsSuccess)
                return Result.Failure(lTupleRes.Error);

            await lTupleRes.Value.Item1.ParallelForEachAsync(
                    _maxDegreeOfParallelism,
                    lUser => lUser.RevokeRoleAsync(lTupleRes.Value.Item2),
                    aCancellationToken);

            return Result.Success();

        }

        /// <summary>
        /// Commands this discord bot to create a new Role in the context server.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aRoleName">string that will name the new Role.</param>
        /// <returns><see cref="Result{string}"/> with information about success or fail on this operation and the Id of the new Role if succeed.</returns>
        public async Task<Result<string>> CreateRole(string aRoleName, CancellationToken aCancellationToken = default)
        {
            var lGuildRes = await this.TryGetDiscordGuildFromConfigAsync(aCancellationToken);
            if (!lGuildRes.IsSuccess)
                return Result.Failure<string>(lGuildRes.Error);

            var lRole = await lGuildRes.Value.CreateRoleAsync(aRoleName, aCancellationToken);
            if (lRole == null)
                return Result.Failure<string>(DiscordBotErrors.Role.RoleNotCreated);

            return Result.Success(lRole.Id.ToString());

        }

        /// <summary>
        /// Commands this discord bot to create a new Role in the context server.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aRoleName">string that will name the new Role.</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation and the Id of the new Role if succeed.</returns>
        public async Task<Result> DeleteRole(ulong aRoleId, CancellationToken aCancellationToken = default)
        {
            var lGuildRes = await this.TryGetDiscordGuildFromConfigAsync(aCancellationToken);
            if (!lGuildRes.IsSuccess)
                return Result.Failure<string>(lGuildRes.Error);

            await lGuildRes.Value.Roles.FirstOrDefault(r => r.Key == aRoleId).Value.DeleteAsync();

            return Result.Success();

        }

        #endregion

        #region Guild

        /// <summary>
        /// Commands this discord bot to get if a given user has verified account. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <returns>true if the user has verified account, false otherwise</returns>
        public async Task<Result<int>> GetNumberOfOnlineUsers(CancellationToken aCancellationToken = default)
        {
            var lGuildRes = await this.TryGetDiscordGuildFromConfigAsync(aCancellationToken);
            if (!lGuildRes.IsSuccess)
                return Result.Failure<int>(lGuildRes.Error);

            var lMemberList = await lGuildRes.Value.GetAllMembersAsync();
            var lUserCount = lMemberList.Count(x => x.Presence != null
                                                    && x.Presence.Status == UserStatus.Online
                                                    && x.VoiceState?.Channel != null);
            return Result.Success(lUserCount);

        }

        /// <summary>
        /// Commands this discord bot to create a new category in the context server from a given template. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aEventCategoryChannelTemplate"><see cref="EventCategoryChannelTemplate"/> template to follow on creating the new category.</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation.</returns>
        public async Task<Result<ulong>> CreateCategoryFromTemplate(CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
        {
            var lGuildRes = await this.TryGetDiscordGuildFromConfigAsync(aCancellationToken);
            if (!lGuildRes.IsSuccess)
                return Result.Failure<ulong>(lGuildRes.Error);

            var lEveryoneRoleRes = await this.TryGetDiscordRoleAsync(lGuildRes.Value.Id, aCancellationToken);
            if (!lEveryoneRoleRes.IsSuccess)
                return Result.Failure<ulong>(lEveryoneRoleRes.Error);

            var lMakePrivateDiscordOverwriteBuilder = new DiscordOverwriteBuilder[] { new DiscordOverwriteBuilder(lEveryoneRoleRes.Value).Deny(Permissions.AccessChannels) };
            aCancellationToken.ThrowIfCancellationRequested();
            var lNewCategory = await lGuildRes.Value.CreateChannelCategoryAsync(aCategoryChannelTemplate.Name, lMakePrivateDiscordOverwriteBuilder);
            await lGuildRes.Value.GetChannelsAsync();
            await aCategoryChannelTemplate.ChannelList.ParallelForEachAsync(
                _maxDegreeOfParallelism,
                x => lGuildRes.Value.CreateChannelAsync(x.Name, x.ChannelType, position: x.Position, parent: lNewCategory, overwrites: lMakePrivateDiscordOverwriteBuilder),
                aCancellationToken);

            return Result.Success(lNewCategory.Id);

        }

        /// <summary>
        /// Gets a valid Id from <see cref="DiscordChannel"/> that is category if exist.
        /// </summary>
        /// <param name="aDiscordCategoryName"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="ulong"/> with valid DiscordChannel Id or default ulong value.</returns>
        public async Task<Result<string>> GetExistingCategoryId(string aDiscordCategoryName, CancellationToken aCancellationToken = default)
        {
            var lGuildRes = await this.TryGetDiscordGuildFromConfigAsync(aCancellationToken);
            if (!lGuildRes.IsSuccess)
                return Result.Failure<string>(lGuildRes.Error);

            return Result.Success(
                (await lGuildRes.Value.GetChannelsAsync())
                .FirstOrDefault(channel => channel.IsCategory
                                && channel.Name == aDiscordCategoryName)
                .Id.ToString());
        }

        /// <summary>
        /// Synchronizes an existing <see cref="DiscordChannel"/> with the given <see cref="CategoryChannelTemplate"/> template, removing not matching channels and adding missing ones.
        /// </summary>
        /// <param name="aDiscordCategoryId"></param>
        /// <param name="aCategoryChannelTemplate"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns>awaitable <see cref="Task"/> with <see cref="Result"/> informing about success or failure in operation.</returns>
        public async Task<Result> SyncExistingCategoryWithTemplate(ulong aDiscordCategoryId, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
        {
            var lGuildRes = await this.TryGetDiscordGuildFromConfigAsync(aCancellationToken);
            if (!lGuildRes.IsSuccess)
                return Result.Failure(lGuildRes.Error);

            var lExistingCategory = lGuildRes.Value.GetChannel(aDiscordCategoryId);
            if(lExistingCategory == null)
                return Result.Failure(DiscordBotErrors.Channel.NotFoundId);

            await lExistingCategory.SyncExistingCategoryWithTemplate_Delete(aCategoryChannelTemplate, aCancellationToken);
            await lExistingCategory.SyncExistingCategoryWithTemplate_Create(aCategoryChannelTemplate, aCancellationToken);

            return Result.Success();

        }

        /// <summary>
        /// Commands this discord bot delete a given category channel and all inner channels. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aEventCategorylId">Id of the category channel</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation.</returns>
        public async Task<Result> DeleteCategoryFromId(ulong aEventCategorylId, CancellationToken aCancellationToken = default)
        {
            var lGuildRes = await this.TryGetDiscordGuildFromConfigAsync(aCancellationToken);
            if (!lGuildRes.IsSuccess)
                return Result.Failure(lGuildRes.Error);

            var lChannelRes = await this.TryGetDiscordChannelAsync(aEventCategorylId, aCancellationToken);
            if (!lChannelRes.IsSuccess)
                return Result.Failure(lChannelRes.Error);

            await lChannelRes.Value.Children.ParallelForEachAsync(
                _maxDegreeOfParallelism,
                x => x.DeleteAsync("Event finished"),
                aCancellationToken);
            await lChannelRes.Value.DeleteAsync("Event finished");
            return Result.Success();

        }

        /// <summary>
        /// Commands this discord bot add a given list of users to a given category channel and all inner channels. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aCategoryId">Id of the category channel</param>
        /// /// <param name="aUserFullHandleList">List of discord full handles</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation.</returns>
        public async Task<Result> AddMemberListToChannel(ulong aChannelId, string[] aUserFullHandleList, CancellationToken aCancellationToken = default)
        {
            var lGuildRes = await this.TryGetDiscordGuildFromConfigAsync(aCancellationToken);
            if (!lGuildRes.IsSuccess)
                return Result.Failure(lGuildRes.Error);

            var lChannelRes = await this.TryGetDiscordChannelAsync(aChannelId, aCancellationToken);
            if (!lChannelRes.IsSuccess)
                return Result.Failure(lChannelRes.Error);

            var lMemberListRes = await lGuildRes.Value.TryGetGuildMemberListFromHandlesAsync(aUserFullHandleList, aCancellationToken);
            if (!lMemberListRes.IsSuccess)
                return Result.Failure(lMemberListRes.Error);

            var lOverWriteBuilderList = lMemberListRes.Value
                .Select(x => new DiscordOverwriteBuilder(x).Allow(Permissions.AccessChannels | Permissions.UseVoice))
                .ToList();//Materialize list to avoid double materializing below 

            //As far as lChannelRes.Value.PermissionOverwrites is write-only, we have to copy current channel's overwrites and add them to the new ones we want to add.
            await lChannelRes.Value.PermissionOverwrites.ParallelForEachAsync(
                _maxDegreeOfParallelism,
                x => lOverWriteBuilderList.UpdateBuilderOverwrites(x),
                aCancellationToken);

            //We override the overrites with our new ones plus the already existing ones as we aded them to lOverWriteBuilderList below.
            await lChannelRes.Value.Children.ParallelForEachAsync(
                _maxDegreeOfParallelism,
                x => x.ModifyAsync(x => x.PermissionOverwrites = lOverWriteBuilderList),
                aCancellationToken);
            return Result.Success();

        }

        #endregion

    }
}
