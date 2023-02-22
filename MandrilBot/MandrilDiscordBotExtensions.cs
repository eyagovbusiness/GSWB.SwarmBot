using DSharpPlus;
using DSharpPlus.Entities;
using TheGoodFramework.CA.Domain.Primitives.Result;
using TheGoodFramework.Extensions;

namespace MandrilBot
{
    public static class MandrilDiscordBotExtensions
    {

        private static readonly byte _maxDegreeOfParallelism = Convert.ToByte(Math.Ceiling((Environment.ProcessorCount * 0.75)));

        #region Verify

        /// <summary>
        /// Commands this discord bot to get if exist a user account with the given Id.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns>True if an user was found with the given Id, false otherwise</returns>
        public static async Task<Result<bool>> ExistUser(this MandrilDiscordBot aBot, ulong aUserId)
        {
            try
            {
                return Result.Success(await aBot.Client.GetUserAsync(aUserId) != null);
            }
            catch (Exception)
            {
                //Add logging the exception
                return Result.Failure<bool>(DiscordBotErrors.BadRequest);
            }

        }

        /// <summary>
        /// Commands this discord bot to get if a given user has verified account. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aFullDiscordHandle">string representing the full discord Handle with format {Username}#{Discriminator} of the user.</param>
        /// <returns>true if the user has verified account, false otherwise</returns>
        public static async Task<Result<bool>> IsUserVerified(this MandrilDiscordBot aBot, ulong aUserId)
        {
            try
            {
                var lUser = await aBot.Client.GetUserAsync(aUserId);
                if (lUser == null)
                    return Result.Failure<bool>(DiscordBotErrors.User.NotFoundId);
                return Result.Success(lUser.Verified.GetValueOrDefault(false));
            }
            catch (Exception)
            {
                //Add logging the exception
                return Result.Failure<bool>(DiscordBotErrors.Role.OneUserRoleNotAssigned);
            }

        }

        /// <summary>
        /// Commands this discord bot to get the date of creation of the given user account.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aUserId">Id of the User.</param>
        /// <returns><see cref="DateTimeOffset"/> with the date of creation of the given user account.</returns>
        public static async Task<Result<DateTimeOffset>> GetUserCreationDate(this MandrilDiscordBot aBot, ulong aUserId)
        {
            try
            {
                var lUser = await aBot.Client.GetUserAsync(aUserId);
                if (lUser == null)
                    return Result.Failure<DateTimeOffset>(DiscordBotErrors.User.NotFoundId);
                return Result.Success(lUser.CreationTimestamp);
            }
            catch (Exception)
            {
                //Add logging the exception
                return Result.Failure<DateTimeOffset>(DiscordBotErrors.BadRequest);
            }

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
        public static async Task<Result> AssignRoleToUser(this MandrilDiscordBot aBot, ulong aRoleId, string aFullDiscordHandle)
        {
            try
            {
                var lRole = await TryGetDiscordRoleAsync(aBot, aRoleId);
                var lMember = await TryGetDiscordMemberAsync(aBot, aFullDiscordHandle);

                if (!lMember.IsSuccess)
                    Result.Failure(DiscordBotErrors.Member.NotFoundHandle);

                await lMember.Value.GrantRoleAsync(lRole.Value);
                return Result.Success();
            }
            catch (Exception)
            {
                //Add logging the exception
                return Result.Failure(DiscordBotErrors.Role.OneUserRoleNotAssigned);
            }

        }

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every user in the given list from the server in this context.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aRoleId">Id of the role to assign in this server to the users.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {Username}#{Discriminator} of the users.</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation.</returns>
        public static async Task<Result> AssignRoleToUserList(this MandrilDiscordBot aBot, ulong aRoleId, string[] aFullHandleList)
        {
            if (aFullHandleList.IsNullOrEmpty())
                return Result.Failure(DiscordBotErrors.List.Empty);
            try
            {
                var lTupleRes = await TryGetMemberListAndRoleAsync(aBot, aRoleId, aFullHandleList);
                if (!lTupleRes.IsSuccess)
                    return Result.Failure(lTupleRes.Error);

                await lTupleRes.Value.Item1.ParallelForEachAsync(
                        _maxDegreeOfParallelism,
                        lUser => lUser.GrantRoleAsync(lTupleRes.Value.Item2));

                return Result.Success();
            }
            catch (Exception)
            {
                //Add logging the exception
                return Result.Failure(DiscordBotErrors.Role.OneUserRoleNotAssigned);
            }

        }

        /// <summary>
        /// Commands this discord bot to assign a given Discord Role to every user in the given list from the server in this context.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aRoleId">Id of the role to assign in this server to the users.</param>
        /// <param name="aFullHandleList">Array of string representing the full discord Handle with format {Username}#{Discriminator} of the users.</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation.</returns>
        public static async Task<Result> RevokeRoleToUserList(this MandrilDiscordBot aBot, ulong aRoleId, string[] aFullHandleList)
        {
            if (aFullHandleList.IsNullOrEmpty())
                return Result.Failure(DiscordBotErrors.List.Empty);
            try
            {
                var lTupleRes = await TryGetMemberListAndRoleAsync(aBot, aRoleId, aFullHandleList);
                if (!lTupleRes.IsSuccess)
                    return Result.Failure(lTupleRes.Error);

                await lTupleRes.Value.Item1.ParallelForEachAsync(
                        _maxDegreeOfParallelism,
                        lUser => lUser.RevokeRoleAsync(lTupleRes.Value.Item2));

                return Result.Success();
            }
            catch (Exception)
            {
                //Add logging the exception
                return Result.Failure(DiscordBotErrors.Role.OneUserRoleNotAssigned);
            }

        }

        /// <summary>
        /// Commands this discord bot to create a new Role in the context server.
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aRoleName">string that will name the new Role.</param>
        /// <returns><see cref="Result{ulong}"/> with information about success or fail on this operation and the Id of the new Role if succeed.</returns>
        public static async Task<Result<ulong>> CreateRole(this MandrilDiscordBot aBot, string aRoleName)
        {
            try
            {
                var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
                if (!lGuildRes.IsSuccess)
                    return Result.Failure<ulong>(lGuildRes.Error);

                var lRole = await lGuildRes.Value?.CreateRoleAsync(aRoleName);
                if (lRole == null)
                    return Result.Failure<ulong>(DiscordBotErrors.Role.RoleNotCreated);

                return Result.Success(lRole.Id);
            }
            catch (Exception)
            {
                //Add logging the exception
                return Result.Failure<ulong>(DiscordBotErrors.Role.RoleNotCreated);
            }

        }

        #endregion

        #region Guild

        /// <summary>
        /// Commands this discord bot to get if a given user has verified account. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <returns>true if the user has verified account, false otherwise</returns>
        public static async Task<Result<int>> GetNumberOfOnlineUsers(this MandrilDiscordBot aBot)
        {
            try
            {
                var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
                if (!lGuildRes.IsSuccess)
                    return Result.Failure<int>(lGuildRes.Error);

                var lMemberList = await lGuildRes.Value.GetAllMembersAsync();
                var lUserCount = lMemberList.Count(x => x.Presence != null
                                                        && x.Presence.Status == UserStatus.Online
                                                        && x.VoiceState?.Channel != null);
                return Result.Success(lUserCount);
            }
            catch (Exception)
            {
                //Add logging the exception
                return Result.Failure<int>(DiscordBotErrors.BadRequest);
            }

        }

        /// <summary>
        /// Commands this discord bot to create a new category in the context server from a given template. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aEventCategoryChannelTemplate"><see cref="EventCategoryChannelTemplate"/> template to follow on creating the new category.</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation.</returns>
        public static async Task<Result<ulong>> CreateCategoryFromTemplate(this MandrilDiscordBot aBot, CategoryChannelTemplate aCategoryChannelTemplate)
        {
            try
            {
                var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
                if (!lGuildRes.IsSuccess)
                    return Result.Failure<ulong>(lGuildRes.Error);

                var lEveryoneRoleRes = await TryGetDiscordRoleAsync(aBot, lGuildRes.Value.Id);
                if (!lEveryoneRoleRes.IsSuccess)
                    return Result.Failure<ulong>(lEveryoneRoleRes.Error);


                var lMakePrivateDiscordOverwriteBuilder = new DiscordOverwriteBuilder[] { new DiscordOverwriteBuilder(lEveryoneRoleRes.Value).Deny(Permissions.AccessChannels) };
                var lNewCategory = await lGuildRes.Value.CreateChannelCategoryAsync(aCategoryChannelTemplate.Name, lMakePrivateDiscordOverwriteBuilder);

                //MakePrivate
                //await lNewCategory.PermissionOverwrites.FirstOrDefault().UpdateAsync(deny: Permissions.AccessChannels | Permissions.UseVoice, allow: Permissions.None);

                await aCategoryChannelTemplate.ChannelList.ParallelForEachAsync(
                    _maxDegreeOfParallelism,
                    x => lGuildRes.Value.CreateChannelAsync(x.Name, x.ChannelType, position: x.Position, parent: lNewCategory, overwrites: lMakePrivateDiscordOverwriteBuilder));
                return Result.Success(lNewCategory.Id);

            }
            catch (Exception)
            {
                //Add logging the exception
                return Result.Failure<ulong>(DiscordBotErrors.BadRequest);
            }

        }

        /// <summary>
        /// Commands this discord bot delete a given category channel and all inner channels. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aEventCategorylId">Id of the category channel</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation.</returns>
        public static async Task<Result> DeleteCategoryFromId(this MandrilDiscordBot aBot, ulong aEventCategorylId)
        {
            try
            {
                var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
                if (!lGuildRes.IsSuccess)
                    return Result.Failure(lGuildRes.Error);

                var lChannelRes = await TryGetDiscordChannelAsync(aBot, aEventCategorylId);
                if (!lChannelRes.IsSuccess)
                    return Result.Failure(lChannelRes.Error);

                await lChannelRes.Value.Children.ParallelForEachAsync(
                    _maxDegreeOfParallelism,
                    x => x.DeleteAsync("Event finished"));
                await lChannelRes.Value.DeleteAsync("Event finished");
                return Result.Success();
            }
            catch (Exception)
            {
                //Add logging the exception
                return Result.Failure(DiscordBotErrors.BadRequest);
            }

        }

        /// <summary>
        /// Commands this discord bot add a given list of users to a given category channel and all inner channels. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aCategoryId">Id of the category channel</param>
        /// /// <param name="aUserFullHandleList">List of discord full handles</param>
        /// <returns><see cref="Result"/> with information about success or fail on this operation.</returns>
        public static async Task<Result> AddUserListToChannel(this MandrilDiscordBot aBot, ulong aChannelId, string[] aUserFullHandleList)
        {
            try
            {
                var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
                if (!lGuildRes.IsSuccess)
                    return Result.Failure(lGuildRes.Error);

                var lChannelRes = await aBot.TryGetDiscordChannelAsync(aChannelId);
                if (!lChannelRes.IsSuccess)
                    return Result.Failure(lChannelRes.Error);

                var lMemberListRes = await lGuildRes?.Value.TryGetGuildMemberListFromHandlesAsync(aUserFullHandleList);
                if (!lMemberListRes.IsSuccess)
                    return Result.Failure(lMemberListRes.Error);

                var lOverWriteBuilderList = lMemberListRes.Value
                    .Select(x => new DiscordOverwriteBuilder(x).Allow(Permissions.AccessChannels | Permissions.UseVoice))
                    .ToList();//Materialize list to avoid double materializing below 

                //As far as lChannelRes.Value.PermissionOverwrites is write-only, we have to copy current channel's overwrites and add them to the new ones we want to add.
                await lChannelRes.Value.PermissionOverwrites.ParallelForEachAsync(
                    _maxDegreeOfParallelism,
                    x => lOverWriteBuilderList.UpdateBuilderOverwrites(x));

                lOverWriteBuilderList.Reverse();

                //we override the overrites with our new ones plus the already existing ones as we aded them to lOverWriteBuilderList below.
                await lChannelRes.Value.ModifyAsync(x => x.PermissionOverwrites = lOverWriteBuilderList);
                await lChannelRes.Value.Children.ParallelForEachAsync(_maxDegreeOfParallelism,
                    x => x.ModifyAsync(x => x.PermissionOverwrites = lOverWriteBuilderList));
                return Result.Success();
            }
            catch (Exception)
            {
                //Add logging the exception
                return Result.Failure(DiscordBotErrors.BadRequest);
            }

        }

        #endregion

        #region Internal

        internal static async Task<Result<DiscordGuild>> TryGetDiscordGuildFromConfigAsync(this MandrilDiscordBot aBot)
        {
            try
            {
                var lGuild = await aBot.Client?.GetGuildAsync(aBot._config.DiscordTargetGuildId);
                return lGuild == null
                    ? Result.Failure<DiscordGuild>(DiscordBotErrors.Guild.NotFoundId)
                    : Result.Success(lGuild);
            }
            catch { return Result.Failure<DiscordGuild>(DiscordBotErrors.BadRequest); }
        }

        internal static async Task<Result<DiscordRole>> TryGetDiscordRoleAsync(this MandrilDiscordBot aBot, ulong aRoleId)
        {
            try
            {
                var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
                if (!lGuildRes.IsSuccess)
                    return Result.Failure<DiscordRole>(lGuildRes.Error);

                var lRole = lGuildRes.Value?.GetRole(aRoleId);
                return lRole == null
                    ? Result.Failure<DiscordRole>(DiscordBotErrors.Role.NotFoundId)
                    : Result.Success(lRole);
            }
            catch { return Result.Failure<DiscordRole>(DiscordBotErrors.BadRequest); }
        }

        internal static async Task<Result<DiscordChannel>> TryGetDiscordChannelAsync(this MandrilDiscordBot aBot, ulong aChannelId)
        {
            try
            {
                var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
                if (!lGuildRes.IsSuccess)
                    return Result.Failure<DiscordChannel>(lGuildRes.Error);

                var lChannel = lGuildRes.Value?.GetChannel(aChannelId);
                return lChannel == null
                    ? Result.Failure<DiscordChannel>(DiscordBotErrors.Role.NotFoundId)
                    : Result.Success(lChannel);
            }
            catch { return Result.Failure<DiscordChannel>(DiscordBotErrors.BadRequest); }
        }

        internal static async Task<Result<DiscordMember>> TryGetDiscordMemberAsync(this MandrilDiscordBot aBot, string aFullDiscordHandle)
        {
            try
            {
                var lDiscordHandleParts = aFullDiscordHandle.Split('#');
                var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
                if (!lGuildRes.IsSuccess)
                    return Result.Failure<DiscordMember>(lGuildRes.Error);

                var lAllMemberList = await lGuildRes.Value?.GetAllMembersAsync();
                var lMember = lAllMemberList?.FirstOrDefault(x => x.Username == lDiscordHandleParts[0] && x.Discriminator == lDiscordHandleParts[1]);
                return lMember == null
                    ? Result.Failure<DiscordMember>(DiscordBotErrors.Role.NotFoundId)
                    : Result.Success(lMember);
            }
            catch { return Result.Failure<DiscordMember>(DiscordBotErrors.BadRequest); }
        }

        internal static async Task<Result<Tuple<IEnumerable<DiscordMember>, DiscordRole>>> TryGetMemberListAndRoleAsync(this MandrilDiscordBot aBot, ulong aRoleId, string[] aFullHandleList)
        {
            if (aFullHandleList.IsNullOrEmpty())
                return Result.Failure<Tuple<IEnumerable<DiscordMember>, DiscordRole>>(DiscordBotErrors.List.Empty);
            try
            {
                var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
                if (!lGuildRes.IsSuccess)
                    return Result.Failure<Tuple<IEnumerable<DiscordMember>, DiscordRole>>(lGuildRes.Error);

                var lRoleRes = await aBot.TryGetDiscordRoleAsync(aRoleId);
                if (!lRoleRes.IsSuccess)
                    return Result.Failure<Tuple<IEnumerable<DiscordMember>,
                                                            DiscordRole>>(
                                                                            lRoleRes.Error);

                var lMemberListRes = await lGuildRes.Value.TryGetGuildMemberListFromHandlesAsync(aFullHandleList);//Materialize list to avoid double materializing below 
                if (!lMemberListRes.IsSuccess)
                    return Result.Failure<Tuple<IEnumerable<DiscordMember>,
                                                            DiscordRole>>(
                                                                            lMemberListRes.Error);

                return Result.Success(new Tuple<IEnumerable<DiscordMember>,
                                                            DiscordRole>(
                                                                            lMemberListRes.Value, lRoleRes.Value));
            }
            catch { return Result.Failure<Tuple<IEnumerable<DiscordMember>, DiscordRole>>(DiscordBotErrors.BadRequest); }
        }

        internal static async Task<Result<IEnumerable<DiscordMember>>> TryGetGuildMemberListFromHandlesAsync(this DiscordGuild aGuild, string[] aMemberFullHandleList)
        {
            try
            {
                var lDiscordHandleParts = aMemberFullHandleList
                    .Union(aMemberFullHandleList)
                    .Select(x => x.Split('#'));

                var lAllMemberList = await aGuild.GetAllMembersAsync();

                var lSelectedMemberList = lDiscordHandleParts
                              .Select(
                                x => lAllMemberList
                                     .FirstOrDefault(y => y != null && y.Username == x[0] && y.Discriminator == x[1])).ToList();

                if (lSelectedMemberList.Any(x => x == null))
                    return Result.Failure<IEnumerable<DiscordMember>>(DiscordBotErrors.Member.OneNotFoundHandle);

                return Result.Success<IEnumerable<DiscordMember>>(lSelectedMemberList);
            }
            catch { return Result.Failure<IEnumerable<DiscordMember>>(DiscordBotErrors.BadRequest); }
        }

        /// <summary>
        /// Returns a Task that adds to this list of DiscordOverwriteBuilder a new builder from a current DiscordOverwrite.
        /// </summary>
        /// <param name="aDiscordOverwriteBuilderList">List of DiscordOverwriteBuilder.</param>
        /// <param name="aDiscordOverwrite">Current DiscordOverwrite applied to create the new builder from.</param>
        /// <returns>Task that will update this list from a given DiscordOverwrite.</returns>
        internal static async Task UpdateBuilderOverwrites(this List<DiscordOverwriteBuilder> aDiscordOverwriteBuilderList, DiscordOverwrite aDiscordOverwrite)
        {
            switch (aDiscordOverwrite.Type)
            {
                case OverwriteType.Member:
                    {
                        var lMember = await aDiscordOverwrite.GetMemberAsync();
                        var lDiscordOverwriteMemberBuilder = new DiscordOverwriteBuilder(lMember);
                        var lNewDiscordMemberOverwrite = await lDiscordOverwriteMemberBuilder.FromAsync(aDiscordOverwrite);
                        aDiscordOverwriteBuilderList.Add(lNewDiscordMemberOverwrite);
                    }
                    break;
                case OverwriteType.Role:
                    {
                        var lRole = await aDiscordOverwrite.GetRoleAsync();
                        var lDiscordOverwriteRoleBuilder = new DiscordOverwriteBuilder(lRole);
                        var lNewDiscordRoleOverwrite = await lDiscordOverwriteRoleBuilder.FromAsync(aDiscordOverwrite);
                        aDiscordOverwriteBuilderList.Add(lNewDiscordRoleOverwrite);
                    }
                    break;
            }
        }

        #endregion

    }
}
