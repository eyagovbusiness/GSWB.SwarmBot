using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGF.Common.Extensions;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilBot.Handlers
{
    internal static class ChannelsHandler
    {

        internal static async Task<IHttpResult<string>> CreateTemplateChannelsAtmAsync(DiscordGuild aDiscordGuild, DiscordRole aDiscordEveryoneRole, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
        {
            DiscordOverwriteBuilder[] lMakeinternalDiscordOverwriteBuilder = default;
            return await Result.CancellationTokenResultAsync(aCancellationToken)
                   .Tap(_ => lMakeinternalDiscordOverwriteBuilder = new DiscordOverwriteBuilder[] { new DiscordOverwriteBuilder(aDiscordEveryoneRole).Deny(Permissions.AccessChannels) })
                   .Map(_ => aDiscordGuild.CreateChannelCategoryAsync(aCategoryChannelTemplate.Name, lMakeinternalDiscordOverwriteBuilder))
                   .Tap(newCategory => aCategoryChannelTemplate.ChannelList.ParallelForEachAsync(
                                        MandrilDiscordBot._maxDegreeOfParallelism,
                                        x => aDiscordGuild.CreateChannelAsync(x.Name, x.ChannelType, position: x.Position, parent: newCategory, overwrites: lMakeinternalDiscordOverwriteBuilder),
                                        aCancellationToken))
                   .Map(newCategory => newCategory.Id.ToString());

        }

        internal static async Task<IHttpResult<DiscordChannel>> GetDiscordCategoryIdFromName(DiscordGuild aDiscordGiildId, string aDiscordCategoryName, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGiildId.GetChannelsAsync())
                    .Map(discordChannelList => discordChannelList.FirstOrDefault(channel => channel.IsCategory
                                                                 && channel.Name == aDiscordCategoryName))
                    .Verify(discordChannel => discordChannel != null, DiscordBotErrors.Channel.NotFoundName);

        internal static async Task<IHttpResult<DiscordChannel>> GetDiscordChannelFromId(DiscordGuild aDIscordGuild, ulong aDiscordCategoryId, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDIscordGuild.GetChannel(aDiscordCategoryId))
                    .Verify(discordChannel => discordChannel != null, DiscordBotErrors.Channel.NotFoundId);

        /// <summary>
        ///  Performs removal tasks to synchronize a given <see cref="DiscordChannel"/> category internal channels to match a given <see cref="CategoryChannelTemplate"/> template.
        /// </summary>
        /// <param name="aDiscordCategory">Discord channel Category</param>
        /// <param name="aCategoryChannelTemplate">Template of the category.</param>
        /// <param name="aCancellationToken">Dancellation token.</param>
        /// <returns>awaitable <see cref="Task"/></returns>
        internal static async Task<IHttpResult<DiscordChannel>> SyncExistingCategoryWithTemplate_Delete(DiscordChannel aDiscordCategory, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordCategory.Children
                              .Where(channel => !aCategoryChannelTemplate.ChannelList
                                                .Any(template => template.Name == channel.Name))
                              .ToList())
                    .Tap(deleteChannelList => deleteChannelList.ParallelForEachAsync(
                                              MandrilDiscordBot._maxDegreeOfParallelism,
                                              channel => channel.DeleteAsync("This channel's category was assigned to an event and this channel did not match the template."),
                                              aCancellationToken))
                    .Map(_ => aDiscordCategory);

        /// <summary>
        ///  Performs creation tasks to synchronize a given <see cref="DiscordChannel"/> category internal channels to match a given <see cref="CategoryChannelTemplate"/> template.
        /// </summary>
        /// <param name="aDiscordCategory">Discord channel Category</param>
        /// <param name="aCategoryChannelTemplate">Template of the category.</param>
        /// <param name="aCancellationToken">Dancellation token.</param>
        /// <returns>awaitable <see cref="Task"/></returns>
        internal static async Task<IHttpResult<Unit>> SyncExistingCategoryWithTemplate_Create(DiscordChannel aDiscordCategory, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aCategoryChannelTemplate.ChannelList
                                .Where(template => aDiscordCategory.Children
                                                .All(channel => channel.Name != template.Name))
                                .ToList())
                    .Tap(missingTemplateList => missingTemplateList.ParallelForEachAsync(
                                                MandrilDiscordBot._maxDegreeOfParallelism,
                                                template => aDiscordCategory.Guild.CreateChannelAsync(template.Name, template.ChannelType, parent: aDiscordCategory, reason: "A Category channel was assigned to an event and this channel was missing according to the template."),
                                                aCancellationToken))
                    .Map(_ => Unit.Value);

        internal static async Task<IHttpResult<Unit>> DeleteCategoryFromId(DiscordChannel aCategoryToDelete, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Tap(_ => aCategoryToDelete.Children.ParallelForEachAsync(
                            MandrilDiscordBot._maxDegreeOfParallelism,
                            x => x.DeleteAsync("Event finished"),
                            aCancellationToken))
                    .Tap(_ => aCategoryToDelete.DeleteAsync("Event finished"));

    }
}
