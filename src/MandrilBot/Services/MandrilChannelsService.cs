using DSharpPlus;
using DSharpPlus.Entities;
using Mandril.Application;
using Mandril.Application.DTOs;
using MandrilBot.Handelers;
using MandrilBot.Handlers;
using TGF.Common.Extensions;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilBot.Services
{
    /// <summary>
    /// Mandril bot service that gives support to DiscordChannel related operations.
    /// </summary>
    /// <remarks>Depends on <see cref="IMandrilDiscordBot"/>.</remarks>
    public class MandrilChannelsService : IMandrilChannelsService
    {
        private readonly GuildsHandler _guildsHandler;
        public MandrilChannelsService(IMandrilDiscordBot aMandrilDiscordBot)
            => _guildsHandler = new GuildsHandler(aMandrilDiscordBot);

        /// <summary>
        /// Commands the discord bot to create a new category in the context server from the provided template. 
        /// </summary>
        /// <param name="aCategoryChannelTemplate"><see cref="CategoryChannelTemplateDTO"/> template to follow on creating the new category.</param>
        /// <returns><see cref="IHttpResult{ulong}"/> with the Id of the created category channel and information about success or failure on this operation.</returns>
        public async Task<IHttpResult<ulong>> CreateCategoryFromTemplate(CategoryChannelTemplateDTO aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => RolesHandler.GetDiscordRoleAtm(discordGuild, discordGuild.Id, aCancellationToken)
                    .Bind(discordEveryoneRole => ChannelsHandler.CreateTemplateChannelsAtmAsync(discordGuild, discordEveryoneRole, aCategoryChannelTemplate, aCancellationToken)));

        /// <summary>
        /// Gets the first <see cref="DiscordChannel"/> frrom the context server channel list that statisfies the given filter conditions.
        /// </summary>
        /// <param name="aFilterFunc">Filter function to get the required channel.</param>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="DiscordChannel"/></returns>
        public async Task<IHttpResult<DiscordChannel>> GetDiscordChannel(Func<DiscordChannel, bool> aFilterFunc, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => ChannelsHandler.GetDiscordChannel(discordGuild, aFilterFunc, aCancellationToken));

        /// <summary>
        /// Gets a valid Id from <see cref="DiscordChannel"/> that is a Category channel if exist.
        /// </summary>
        /// <param name="aDiscordCategoryName">Name of the category from which to get the Id</param>
        /// <param name="aCancellationToken"></param>
        /// <returns><see cref="IHttpResult{ulong}"/> with valid DiscordChannel Id and information about success or failure on this operation.</returns>
        public async Task<IHttpResult<ulong>> GetExistingCategoryId(string aDiscordCategoryName, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => ChannelsHandler.GetDiscordCategory(discordGuild, channel => channel.Name == aDiscordCategoryName, aCancellationToken))
                    .Map(discordChannel => discordChannel.Id);

        /// <summary>
        /// Synchronizes an existing <see cref="DiscordChannel"/> with the given <see cref="CategoryChannelTemplateDTO"/> template, removing not matching channels and adding missing ones.
        /// </summary>
        /// <param name="aDiscordCategoryId"></param>
        /// <param name="aCategoryChannelTemplate"></param>
        /// <param name="aCancellationToken"></param>
        /// <returns>awaitable <see cref="Task"/> with <see cref="IHttpResult{Unit}"/> informing about success or failure in operation.</returns>
        public async Task<IHttpResult<Unit>> SyncExistingCategoryWithTemplate(ulong aDiscordCategoryId, CategoryChannelTemplateDTO aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => ChannelsHandler.GetDiscordChannelFromId(discordGuild, aDiscordCategoryId))
                    .Bind(discordCategory => ChannelsHandler.SyncExistingCategoryWithTemplate_Delete(discordCategory, aCategoryChannelTemplate, aCancellationToken))
                    .Bind(discordCategory => ChannelsHandler.SyncExistingCategoryWithTemplate_Create(discordCategory, aCategoryChannelTemplate, aCancellationToken));

        /// <summary>
        /// Commands this discord bot delete a given category channel and all inner channels. 
        /// </summary>
        /// <param name="aBot">Current discord bot that will execute the commands.</param>
        /// <param name="aEventCategorylId">Id of the category channel</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> DeleteCategoryFromId(ulong aEventCategorylId, CancellationToken aCancellationToken = default)
            => await _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken)
                    .Bind(discordGuild => ChannelsHandler.GetDiscordChannelFromId(discordGuild, aEventCategorylId))
                    .Bind(discordChannel => ChannelsHandler.DeleteCategoryFromId(discordChannel, aCancellationToken));

        /// <summary>
        /// Commands this discord bot add a given list of users to a given category channel and all inner channels. 
        /// </summary>
        /// /// <param name="aUserFullHandleList">List of discord full handles</param>
        /// <returns><see cref="IHttpResult{Unit}"/> with information about success or fail on this operation.</returns>
        public async Task<IHttpResult<Unit>> AddMemberListToChannel(ulong aChannelId, string[] aUserFullHandleList, CancellationToken aCancellationToken = default)
        {
            DiscordGuild aDiscordGuild = default!;
            DiscordChannel aDiscordChannel = default!;
            return await Result.CancellationTokenResultAsync(aCancellationToken)
            .Bind(_ => _guildsHandler.GetDiscordGuildFromConfigAsync(aCancellationToken))
            .Tap(discordGuild => aDiscordGuild = discordGuild)
            .Bind(discordGuild => ChannelsHandler.GetDiscordChannelFromId(discordGuild, aChannelId, aCancellationToken))
            .Tap(discordGuild => aDiscordChannel = discordGuild)
            .Bind(discordChannel => MembersHandler.GetAllDiscordMemberListAtmAsync(aDiscordGuild, aCancellationToken))
            .Map(discordMemberList => discordMemberList.Select(x => new DiscordOverwriteBuilder(x).Allow(Permissions.AccessChannels | Permissions.UseVoice)).ToList())
            .Tap(discordOverwriteList => aDiscordChannel.PermissionOverwrites.ParallelForEachAsync(
                MandrilDiscordBot._maxDegreeOfParallelism,
                x => MembersHandler.UpdateBuilderOverwrites(discordOverwriteList, x),
                aCancellationToken))
            .Tap(discordOverwriteList => aDiscordChannel.ModifyAsync(x => x.PermissionOverwrites = discordOverwriteList))
            .Tap(discordOverwriteList => aDiscordChannel.Children.ParallelForEachAsync(
                 MandrilDiscordBot._maxDegreeOfParallelism,
                x => x.ModifyAsync(x => x.PermissionOverwrites = discordOverwriteList),
                aCancellationToken))
            .Map(_ => Unit.Value);
        }

    }
}
