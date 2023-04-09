using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TGF.Common.Extensions;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;
using static MandrilBot.DiscordBotErrors;

namespace MandrilBot
{
    public partial class MandrilDiscordBot
    {
        #region Guild atomics

        private async Task<IHttpResult<DiscordGuild>> GetDiscordGuildFromConfigAsync(CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => Client.GetGuildAsync(_botConfiguration.DiscordTargetGuildId))
                    .Verify(discordGuild => discordGuild != null, DiscordBotErrors.Guild.NotFoundId);

        #endregion

        #region Role atomics

        private static async Task<IHttpResult<DiscordRole>> GetDiscordRoleAtm(DiscordGuild aDiscordGuild, ulong aRoleId, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGuild.GetRole(aRoleId))
                    .Verify(discordRole => discordRole != null, DiscordBotErrors.Role.NotFoundId);

        private static async Task<IHttpResult<string>> CreateRoleAtmAsync(DiscordGuild aDiscordGuild, string aRoleName, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGuild.CreateRoleAsync(aRoleName))
                    .Verify(newRole => newRole != null, DiscordBotErrors.Role.RoleNotCreated)
                    .Map(newRole => newRole.Id.ToString());

        private static async Task<IHttpResult<Unit>> DeleteRoleAtmAsync(DiscordGuild aDiscordGuild, ulong aRoleId, CancellationToken aCancellationToken = default)
        => await Result.CancellationTokenResultAsync(aCancellationToken)
                .Tap(_ => aDiscordGuild.Roles.FirstOrDefault(r => r.Key == aRoleId).Value?.DeleteAsync());


        private static async Task<IHttpResult<Unit>> GrantRoleToMemberAtmAsync(DiscordMember aDiscordMember, DiscordRole aDiscordRole, string aReason = null, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Tap(_ => aDiscordMember.GrantRoleAsync(aDiscordRole, aReason));

        private static async Task<IHttpResult<Unit>> GrantRoleToMemberListAtmAsync(IEnumerable<DiscordMember> aDiscordMemberList, DiscordRole aDiscordRole, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                           .Tap(_ => aDiscordMemberList.ParallelForEachAsync(
                                    _maxDegreeOfParallelism,
                                    lMember => lMember.GrantRoleAsync(aDiscordRole),
                                    aCancellationToken));

        private static async Task<IHttpResult<Unit>> RevokeRoleToMemberListAtmAsync(IEnumerable<DiscordMember> aDiscordMemberList, DiscordRole aDiscordRole, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
               .Tap(_ => aDiscordMemberList.ParallelForEachAsync(
                        _maxDegreeOfParallelism,
                        lMember => lMember.RevokeRoleAsync(aDiscordRole),
                        aCancellationToken));

        #endregion

        #region User atomics
        private async Task<IHttpResult<DiscordUser>> GetUserAsync(ulong aUserId)
        {
            try
            {
                return Result.SuccessHttp(await Client.GetUserAsync(aUserId));
            }
            catch(DSharpPlus.Exceptions.NotFoundException)
            {
                return Result.SuccessHttp<DiscordUser>(null);
            }

        }
        private async Task<IHttpResult<DiscordUser>> GetUserAsync(ulong aUserId, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Bind(_ => GetUserAsync(aUserId));
        #endregion

        #region Member atomics

        #region Validators


        private static bool ValidateMemberHandleString(string aFullDiscordHandle)
            => !string.IsNullOrWhiteSpace(aFullDiscordHandle) 
                && Regex.IsMatch(aFullDiscordHandle, @"^[a-zA-Z0-9]{2,32}#\d{4}$");


        private static IHttpResult<Unit> ValidateMemberHandle(string aFullDiscordHandle)
            => ValidateMemberHandleString(aFullDiscordHandle)
                ? Result.SuccessHttp(Unit.Value)
                : Result.Failure<Unit>(DiscordBotErrors.Member.InvalidHandle);

        private static IHttpResult<Unit> ValidateMemberHandleList(string[] aFullDiscordHandleList)
            => aFullDiscordHandleList
               .All(handle => ValidateMemberHandleString(handle))
                    ? Result.SuccessHttp(Unit.Value)
                    : Result.Failure<Unit>(DiscordBotErrors.Member.InvalidHandle);

        #endregion
        private static async Task<DiscordMember> GetDiscordMemberAtmAsync(DiscordGuild aDiscordGuild, string aFullDiscordHandle)
        {
            var lDiscordHandleParts = aFullDiscordHandle.Split('#');
            var lAllMemberList = await aDiscordGuild.GetAllMembersAsync();
            return lAllMemberList?.FirstOrDefault(x => x.Username == lDiscordHandleParts[0] && x.Discriminator == lDiscordHandleParts[1]);
        }

        private static async Task<IHttpResult<DiscordMember>> GetDiscordMemberAtmAsync(DiscordGuild aDiscordGuild, string aFullDiscordHandle, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => GetDiscordMemberAtmAsync(aDiscordGuild, aFullDiscordHandle))
                    .Verify(discordMember => discordMember != null, DiscordBotErrors.Member.NotFoundHandle);

        private static async Task<IHttpResult<ImmutableArray<DiscordMember>>> GetDiscordMemberListAtmAsync(DiscordGuild aDiscordGuild, string[] aMemberFullHandleList, CancellationToken aCancellationToken = default)
        {
            IEnumerable<string[]> lDiscordHandleParts = default;
            return await Result.CancellationTokenResultAsync(aCancellationToken)
                        .Tap(_ => lDiscordHandleParts = aMemberFullHandleList
                                                        .Union(aMemberFullHandleList)
                                                        .Select(x => x.Split('#')))
                        .Bind(_ => GetAllDiscordMemberListAtmAsync(aDiscordGuild, aCancellationToken))
                        .Map(allMemberList => lDiscordHandleParts
                                          .Select(
                                            x => allMemberList
                                                 .FirstOrDefault(y => y != null && y.Username == x[0] && y.Discriminator == x[1]))
                                          .ToImmutableArray())
                        .Verify(selectedMemberList => selectedMemberList.All(m => m != null), DiscordBotErrors.Member.OneNotFoundHandle);

        }

        private static async Task<IHttpResult<ImmutableArray<DiscordMember>>> GetAllDiscordMemberListAtmAsync(DiscordGuild aDiscordGuild, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGuild.GetAllMembersAsync())
                    .Map(discordMemberList => discordMemberList.ToImmutableArray());



        #endregion


        #region Channel atomics
        private static async Task<IHttpResult<string>> CreateTemplateChannelsAtmAsync(DiscordGuild aDiscordGuild, DiscordRole aDiscordEveryoneRole, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
        {
            DiscordOverwriteBuilder[] lMakePrivateDiscordOverwriteBuilder = default;
            return await Result.CancellationTokenResultAsync(aCancellationToken)
                   .Tap(_ => lMakePrivateDiscordOverwriteBuilder = new DiscordOverwriteBuilder[] { new DiscordOverwriteBuilder(aDiscordEveryoneRole).Deny(Permissions.AccessChannels) })
                   .Map(_ => aDiscordGuild.CreateChannelCategoryAsync(aCategoryChannelTemplate.Name, lMakePrivateDiscordOverwriteBuilder))
                   .Tap(newCategory => aCategoryChannelTemplate.ChannelList.ParallelForEachAsync(
                                        _maxDegreeOfParallelism,
                                        x => aDiscordGuild.CreateChannelAsync(x.Name, x.ChannelType, position: x.Position, parent: newCategory, overwrites: lMakePrivateDiscordOverwriteBuilder),
                                        aCancellationToken))
                   .Map(newCategory => newCategory.Id.ToString());

        }

        private static async Task<IHttpResult<DiscordChannel>> GetDiscordCategoryIdFromName(DiscordGuild aDiscordGiildId, string aDiscordCategoryName, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGiildId.GetChannelsAsync())
                    .Map(discordChannelList => discordChannelList.FirstOrDefault(channel => channel.IsCategory
                                                                 && channel.Name == aDiscordCategoryName))
                    .Verify(discordChannel => discordChannel != null, DiscordBotErrors.Channel.NotFoundName);

        private static async Task<IHttpResult<DiscordChannel>> GetDiscordChannelFromId(DiscordGuild aDIscordGuild, ulong aDiscordCategoryId, CancellationToken aCancellationToken = default)
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
        private static async Task<IHttpResult<DiscordChannel>> SyncExistingCategoryWithTemplate_Delete(DiscordChannel aDiscordCategory, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
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
        private static async Task<IHttpResult<Unit>> SyncExistingCategoryWithTemplate_Create(DiscordChannel aDiscordCategory, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
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
        #endregion
    }
}
