namespace MandrilBot
{
    /// <summary>
    /// Internal extension methods to support MandrilDiscordBot public methods.
    /// </summary>
    //internal static partial class MandrilDiscordBotExtensions
    //{
    //    //Sadly DsharpPlus does not support CancellationToken propagation into the library methods, so i have to check the cancellation token manually before every external Discord API call
    //    #region Internal
    //    internal static async Task<Result<DiscordChannel>> TryGetDiscordChannelAsync(this MandrilDiscordBot aBot, ulong aChannelId, CancellationToken aCancellationToken = default)
    //    {
    //        aCancellationToken.ThrowIfCancellationRequested();
    //        var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
    //        if (!lGuildRes.IsSuccess)
    //            return Result.Failure<DiscordChannel>(lGuildRes.Error);

    //        var lChannel = lGuildRes.Value?.GetChannel(aChannelId);
    //        return lChannel == null
    //            ? Result.Failure<DiscordChannel>(DiscordBotErrors.Channel.NotFoundId)
    //            : Result.Success(lChannel);

    //    }


    //    internal static async Task<DiscordUser> GetUserAsync(this MandrilDiscordBot aBot, ulong aUserId, CancellationToken aCancellationToken = default)
    //    {
    //        aCancellationToken.ThrowIfCancellationRequested();
    //        return await aBot.Client.GetUserAsync(aUserId);
    //    }

    //    internal static async Task GrantRoleAsync(this DiscordMember aMember, DiscordRole aRole, string aReason = null, CancellationToken aCancellationToken = default)
    //    {
    //        aCancellationToken.ThrowIfCancellationRequested();
    //        await aMember.GrantRoleAsync(aRole, aReason);
    //    }

    //    internal static async Task<Result<Tuple<IEnumerable<DiscordMember>, DiscordRole>>> TryGetMemberListAndRoleAsync(this MandrilDiscordBot aBot, ulong aRoleId, string[] aFullHandleList, CancellationToken aCancellationToken = default)
    //    {
    //        aCancellationToken.ThrowIfCancellationRequested();
    //        if (aFullHandleList.IsNullOrEmpty())
    //            return Result.Failure<Tuple<IEnumerable<DiscordMember>, DiscordRole>>(DiscordBotErrors.List.Empty);

    //        var lGuildRes = await TryGetDiscordGuildFromConfigAsync(aBot);
    //        if (!lGuildRes.IsSuccess)
    //            return Result.Failure<Tuple<IEnumerable<DiscordMember>, DiscordRole>>(lGuildRes.Error);

    //        var lRoleRes = await aBot.TryGetDiscordRoleAsync(aRoleId);
    //        if (!lRoleRes.IsSuccess)
    //            return Result.Failure<Tuple<IEnumerable<DiscordMember>,
    //                                                    DiscordRole>>(
    //                                                                lRoleRes.Error);

    //        var lMemberListRes = await lGuildRes.Value.TryGetGuildMemberListFromHandlesAsync(aFullHandleList);//Materialize list to avoid double materializing below 
    //        if (!lMemberListRes.IsSuccess)
    //            return Result.Failure<Tuple<IEnumerable<DiscordMember>,
    //                                                    DiscordRole>>(
    //                                                                lMemberListRes.Error);

    //        return Result.Success(new Tuple<IEnumerable<DiscordMember>,
    //                                                    DiscordRole>(
    //                                                                lMemberListRes.Value, lRoleRes.Value));

    //    }

    //    internal static async Task<Result<IEnumerable<DiscordMember>>> TryGetGuildMemberListFromHandlesAsync(this DiscordGuild aGuild, string[] aMemberFullHandleList, CancellationToken aCancellationToken = default)
    //    {
    //        aCancellationToken.ThrowIfCancellationRequested();

    //        if (aMemberFullHandleList.First() == "All")
    //        {
    //            var lAllGuilMemberList = await aGuild.GetAllMembersAsync();
    //            return Result.Success(lAllGuilMemberList.ToArray() as IEnumerable<DiscordMember>);
    //        }

    //        var lDiscordHandleParts = aMemberFullHandleList
    //            .Union(aMemberFullHandleList)
    //            .Select(x => x.Split('#'));

    //        var lAllMemberList = await aGuild.GetAllMembersAsync();
    //        var lSelectedMemberList = lDiscordHandleParts
    //                      .Select(
    //                        x => lAllMemberList
    //                             .FirstOrDefault(y => y != null && y.Username == x[0] && y.Discriminator == x[1])).ToList();

    //        if (lSelectedMemberList.Any(x => x == null))
    //            return Result.Failure<IEnumerable<DiscordMember>>(DiscordBotErrors.Member.OneNotFoundHandle);

    //        return Result.Success<IEnumerable<DiscordMember>>(lSelectedMemberList);

    //    }

    //    /// <summary>
    //    /// Returns a Task that adds to this list of DiscordOverwriteBuilder a new builder from a current DiscordOverwrite.
    //    /// </summary>
    //    /// <param name="aDiscordOverwriteBuilderList">List of DiscordOverwriteBuilder.</param>
    //    /// <param name="aDiscordOverwrite">Current DiscordOverwrite applied to create the new builder from.</param>
    //    /// <returns>Task that will update this list from a given DiscordOverwrite.</returns>
    //    internal static async Task UpdateBuilderOverwrites(this List<DiscordOverwriteBuilder> aDiscordOverwriteBuilderList, DiscordOverwrite aDiscordOverwrite, CancellationToken aCancellationToken = default)
    //    {
    //        aCancellationToken.ThrowIfCancellationRequested();
    //        switch (aDiscordOverwrite.Type)
    //        {
    //            case OverwriteType.Member:
    //                {
    //                    var lMember = await aDiscordOverwrite.GetMemberAsync();
    //                    var lDiscordOverwriteMemberBuilder = new DiscordOverwriteBuilder(lMember);
    //                    var lNewDiscordMemberOverwrite = await lDiscordOverwriteMemberBuilder.FromAsync(aDiscordOverwrite);
    //                    aDiscordOverwriteBuilderList.Add(lNewDiscordMemberOverwrite);
    //                }
    //                break;
    //            case OverwriteType.Role:
    //                {
    //                    var lRole = await aDiscordOverwrite.GetRoleAsync();
    //                    var lDiscordOverwriteRoleBuilder = new DiscordOverwriteBuilder(lRole);
    //                    var lNewDiscordRoleOverwrite = await lDiscordOverwriteRoleBuilder.FromAsync(aDiscordOverwrite);
    //                    aDiscordOverwriteBuilderList.Add(lNewDiscordRoleOverwrite);
    //                }
    //                break;
    //        }

    //    }

    //    /// <summary>
    //    ///  Performs removal tasks to synchronize a given <see cref="DiscordChannel"/> category internal channels to match a given <see cref="CategoryChannelTemplate"/> template.
    //    /// </summary>
    //    /// <param name="aDiscordCategory">Discord channel Category</param>
    //    /// <param name="aCategoryChannelTemplate">Template of the category.</param>
    //    /// <param name="aMaxDegreeOfParallelism">Max degree of parallelism to use in this method</param>
    //    /// <param name="aCancellationToken">Dancellation token.</param>
    //    /// <returns>awaitable <see cref="Task"/></returns>
    //    internal static async Task SyncExistingCategoryWithTemplate_Delete(this DiscordChannel aDiscordCategory, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
    //    {
    //        var lDeleteChannelList = aDiscordCategory.Children
    //                .Where(channel => !aCategoryChannelTemplate.ChannelList
    //                                                            .Any(template => template.Name == channel.Name))
    //                .ToList();

    //        aCancellationToken.ThrowIfCancellationRequested();
    //        await lDeleteChannelList.ParallelForEachAsync(
    //              MandrilDiscordBot._maxDegreeOfParallelism,
    //              channel => channel.DeleteAsync("This channel's category was assigned to an event and this channel did not match the template."),
    //              aCancellationToken);
    //    }

    //    /// <summary>
    //    ///  Performs creation tasks to synchronize a given <see cref="DiscordChannel"/> category internal channels to match a given <see cref="CategoryChannelTemplate"/> template.
    //    /// </summary>
    //    /// <param name="aDiscordCategory">Discord channel Category</param>
    //    /// <param name="aCategoryChannelTemplate">Template of the category.</param>
    //    /// <param name="aMaxDegreeOfParallelism">Max degree of parallelism to use in this method</param>
    //    /// <param name="aCancellationToken">Dancellation token.</param>
    //    /// <returns>awaitable <see cref="Task"/></returns>
    //    internal static async Task SyncExistingCategoryWithTemplate_Create(this DiscordChannel aDiscordCategory, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
    //    {

    //        var lMissingTemplateList = aCategoryChannelTemplate.ChannelList
    //                                   .Where(template => aDiscordCategory.Children
    //                                                                      .All(channel => channel.Name != template.Name));

    //        if (!lMissingTemplateList.Any())
    //            return;

    //        aCancellationToken.ThrowIfCancellationRequested();
    //        await lMissingTemplateList.ParallelForEachAsync(
    //              MandrilDiscordBot._maxDegreeOfParallelism,
    //              template => aDiscordCategory.Guild.CreateChannelAsync(template.Name, template.ChannelType, parent: aDiscordCategory, reason: "A Category channel was assigned to an event and this channel was missing according to the template."),
    //              aCancellationToken);
    //    }

    //    #endregion

    //}
}
