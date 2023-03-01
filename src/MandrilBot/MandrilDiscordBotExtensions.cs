using DSharpPlus;
using DSharpPlus.Entities;
using System.Data;
using TGF.CA.Domain.Primitives.Result;
using TGF.Common.Extensions;

namespace MandrilBot
{
    /// <summary>
    /// Internal extension methods to support MandrilDiscordBot public methods
    /// </summary>
    internal static class MandrilDiscordBotExtensions
    {
        //Sadly DsharpPlus does not support CancellationToken propagation into the library methods, so i have to check the cancellation token manually before every external Discord API call
        #region Internal

        internal static async Task<Result<DiscordGuild>> TryGetDiscordGuildFromConfigAsync(this MandrilDiscordBot aBot, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            var lGuild = await aBot.Client?.GetGuildAsync(aBot._botConfiguration.DiscordTargetGuildId);
            return lGuild == null
                ? Result.Failure<DiscordGuild>(DiscordBotErrors.Guild.NotFoundId)
                : Result.Success(lGuild);

        }

        internal static async Task<Result<DiscordRole>> TryGetDiscordRoleAsync(this MandrilDiscordBot aBot, ulong aRoleId, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
            if (!lGuildRes.IsSuccess)
                return Result.Failure<DiscordRole>(lGuildRes.Error);

            var lRole = lGuildRes.Value?.GetRole(aRoleId);
            return lRole == null
                ? Result.Failure<DiscordRole>(DiscordBotErrors.Role.NotFoundId)
                : Result.Success(lRole);

        }

        internal static async Task<Result<DiscordChannel>> TryGetDiscordChannelAsync(this MandrilDiscordBot aBot, ulong aChannelId, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
            if (!lGuildRes.IsSuccess)
                return Result.Failure<DiscordChannel>(lGuildRes.Error);

            var lChannel = lGuildRes.Value?.GetChannel(aChannelId);
            return lChannel == null
                ? Result.Failure<DiscordChannel>(DiscordBotErrors.Role.NotFoundId)
                : Result.Success(lChannel);

        }

        internal static async Task<Result<DiscordMember>> TryGetDiscordMemberAsync(this MandrilDiscordBot aBot, string aFullDiscordHandle, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
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

        internal static async Task<DiscordRole> CreateRoleAsync(this DiscordGuild aGuild, string aRoleName, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            return await aGuild.CreateRoleAsync(aRoleName);
        }

        internal static async Task<DiscordUser> GetUserAsync(this MandrilDiscordBot aBot, ulong aUserId, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            return await aBot.Client.GetUserAsync(aUserId);
        }

        internal static async Task GrantRoleAsync(this DiscordMember aMember, DiscordRole aRole, string aReason = null, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            await aMember.GrantRoleAsync(aRole, aReason);
        }

        internal static async Task<Result<Tuple<IEnumerable<DiscordMember>, DiscordRole>>> TryGetMemberListAndRoleAsync(this MandrilDiscordBot aBot, ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            if (aFullHandleList.IsNullOrEmpty())
                return Result.Failure<Tuple<IEnumerable<DiscordMember>, DiscordRole>>(DiscordBotErrors.List.Empty);

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

        internal static async Task<Result<IEnumerable<DiscordMember>>> TryGetGuildMemberListFromHandlesAsync(this DiscordGuild aGuild, string[] aMemberFullHandleList, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();

            //TEST
            //if (aMemberFullHandleList.First() == "All")
            //{
            //    var w = await aGuild.GetAllMembersAsync();
            //    return Result.Success(w.ToArray() as IEnumerable<DiscordMember>);
            //}

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

        /// <summary>
        /// Returns a Task that adds to this list of DiscordOverwriteBuilder a new builder from a current DiscordOverwrite.
        /// </summary>
        /// <param name="aDiscordOverwriteBuilderList">List of DiscordOverwriteBuilder.</param>
        /// <param name="aDiscordOverwrite">Current DiscordOverwrite applied to create the new builder from.</param>
        /// <returns>Task that will update this list from a given DiscordOverwrite.</returns>
        internal static async Task UpdateBuilderOverwrites(this List<DiscordOverwriteBuilder> aDiscordOverwriteBuilderList, DiscordOverwrite aDiscordOverwrite, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
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
