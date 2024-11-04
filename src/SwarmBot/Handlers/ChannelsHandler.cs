﻿using DSharpPlus;
using DSharpPlus.Entities;
using Common.Application.DTOs.Discord;
using TGF.Common.Extensions;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace SwarmBot.Handlers
{
    public static class ChannelsHandler
    {

        public static async Task<IHttpResult<ulong>> CreateTemplateChannelsAtmAsync(DiscordGuild aDiscordGuild, DiscordRole aDiscordEveryoneRole, CategoryChannelTemplateDTO aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
        {
            DiscordOverwriteBuilder[] lMakepublicDiscordOverwriteBuilder = default!;
            return await Result.CancellationTokenResultAsync(aCancellationToken)
                   .Tap(_ => lMakepublicDiscordOverwriteBuilder = [new DiscordOverwriteBuilder(aDiscordEveryoneRole).Deny(Permissions.AccessChannels)])
                   .Map(_ => aDiscordGuild.CreateChannelCategoryAsync(aCategoryChannelTemplate.Name, lMakepublicDiscordOverwriteBuilder))
                   .Tap(newCategory => aCategoryChannelTemplate.ChannelList?.ParallelForEachAsync(
                                        SwarmBotDiscordBot._maxDegreeOfParallelism,
                                        x => aDiscordGuild.CreateChannelAsync(x.Name, (DSharpPlus.ChannelType)x.ChannelType, position: x.Position, parent: newCategory, overwrites: lMakepublicDiscordOverwriteBuilder),
                                        aCancellationToken)!)
                   .Map(newCategory => newCategory.Id);

        }

        public static async Task<IHttpResult<DiscordChannel>> GetDiscordCategory(DiscordGuild aDiscordGiild, Func<DiscordChannel, bool> aFilterFunc, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGiild.GetChannelsAsync())
                    .Map(discordChannelList => discordChannelList.FirstOrDefault(channel => channel.IsCategory && aFilterFunc(channel))!)
                    .Verify(discordChannel => discordChannel != null, DiscordBotErrors.Channel.NotFound);

        public static async Task<IHttpResult<DiscordChannel>> GetDiscordChannelFromId(DiscordGuild aDiscordGuild, ulong aDiscordChannelId, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGuild.GetChannel(aDiscordChannelId))
                    .Verify(discordChannel => discordChannel != null, DiscordBotErrors.Channel.NotFoundId);

        public static async Task<IHttpResult<DiscordChannel>> GetDiscordChannel(DiscordGuild aDiscordGuild, Func<DiscordChannel, bool> aFilterFunc, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGuild.GetChannelsAsync())
                    .Tap(discordChannelList => { if (discordChannelList.IsNullOrEmpty()) throw new Exception("ERROOOOR! GetDiscordChannel was called and DiscordGuild.Channels was null or empty!!! please report this immediatley!"); })
                    .Map(discordChannelList => discordChannelList.FirstOrDefault(channel => aFilterFunc(channel))!)
                    .Verify(discordChannel => discordChannel != null, DiscordBotErrors.Channel.NotFoundName);

        /// <summary>
        ///  Performs removal tasks to synchronize a given <see cref="DiscordChannel"/> category public channels to match a given <see cref="CategoryChannelTemplateDTO"/> template.
        /// </summary>
        /// <param name="aDiscordCategory">Discord channel Category</param>
        /// <param name="aCategoryChannelTemplate">Template of the category.</param>
        /// <param name="aCancellationToken">Dancellation token.</param>
        /// <returns>awaitable <see cref="Task"/></returns>
        public static async Task<IHttpResult<DiscordChannel>> SyncExistingCategoryWithTemplate_Delete(DiscordChannel aDiscordCategory, CategoryChannelTemplateDTO aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordCategory.Children
                              .Where(channel => !aCategoryChannelTemplate.ChannelList!
                                                .Any(template => template.Name == channel.Name))
                              .ToList())
                    .Tap(deleteChannelList => deleteChannelList.ParallelForEachAsync(
                                              SwarmBotDiscordBot._maxDegreeOfParallelism,
                                              channel => channel.DeleteAsync("This channel's category was assigned to an event and this channel did not match the template."),
                                              aCancellationToken))
                    .Map(_ => aDiscordCategory);

        /// <summary>
        ///  Performs creation tasks to synchronize a given <see cref="DiscordChannel"/> category public channels to match a given <see cref="CategoryChannelTemplateDTO"/> template.
        /// </summary>
        /// <param name="aDiscordCategory">Discord channel Category</param>
        /// <param name="aCategoryChannelTemplate">Template of the category.</param>
        /// <param name="aCancellationToken">Dancellation token.</param>
        /// <returns>awaitable <see cref="Task"/></returns>
        public static async Task<IHttpResult<Unit>> SyncExistingCategoryWithTemplate_Create(DiscordChannel aDiscordCategory, CategoryChannelTemplateDTO aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aCategoryChannelTemplate.ChannelList?
                                .Where(template => aDiscordCategory.Children
                                                .All(channel => channel.Name != template.Name))
                                .ToList())
                    .Tap(missingTemplateList => missingTemplateList?.ParallelForEachAsync(
                                                SwarmBotDiscordBot._maxDegreeOfParallelism,
                                                template => aDiscordCategory.Guild.CreateChannelAsync(template.Name, (DSharpPlus.ChannelType)template.ChannelType, parent: aDiscordCategory, reason: "A Category channel was assigned to an event and this channel was missing according to the template."),
                                                aCancellationToken)!)
                    .Map(_ => Unit.Value);

        public static async Task<IHttpResult<Unit>> DeleteCategoryFromId(DiscordChannel aCategoryToDelete, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Tap(_ => aCategoryToDelete.Children.ParallelForEachAsync(
                            SwarmBotDiscordBot._maxDegreeOfParallelism,
                            x => x.DeleteAsync("Event finished"),
                            aCancellationToken))
                    .Tap(_ => aCategoryToDelete.DeleteAsync("Event finished"));

    }
}
