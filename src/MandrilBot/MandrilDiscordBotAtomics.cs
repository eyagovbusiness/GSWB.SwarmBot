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
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using TGF.Common.Extensions;
using TGF.Common.ROP.Result;
using static MandrilBot.DiscordBotErrors;

namespace MandrilBot
{
    public partial class MandrilDiscordBot
    {
        #region Guild atomics

        private async Task<IResult<DiscordGuild>> GetDiscordGuildFromConfigAsync()
        {
            var lGuild = await Client.GetGuildAsync(_botConfiguration.DiscordTargetGuildId);
            return lGuild != null
                ? Result.SuccessHttp(lGuild)
                : Result.Failure<DiscordGuild>(DiscordBotErrors.Guild.NotFoundId.Error);
        }
        private async Task<IResult<DiscordGuild>> GetDiscordGuildFromConfigAsync(CancellationToken aCancellationToken = default)
            =>await Result.CancellationTokenResultAsync(aCancellationToken)
                   .Bind(_ => GetDiscordGuildFromConfigAsync());

        #endregion

        #region Role atomics
        private Task<IResult<DiscordRole>> GetDiscordRoleAtm(DiscordGuild aDiscordGuild, ulong aRoleId, out DiscordRole aDiscordRole, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            aDiscordRole = aDiscordGuild.GetRole(aRoleId);
            return Task.FromResult(aDiscordRole != null
                ? Result.SuccessHttp(aDiscordRole)
                : Result.Failure<DiscordRole>(DiscordBotErrors.Role.NotFoundId));

        }

        private async Task<IResult<string>> CreateRoleAtmAsync(DiscordGuild aDiscordGuild, string aRoleName)
        {
            var lNewRole = await aDiscordGuild.CreateRoleAsync(aRoleName);
            return lNewRole != null
                ? Result.SuccessHttp(lNewRole.Id.ToString())
                : Result.Failure<string>(DiscordBotErrors.Role.RoleNotCreated);

        }
        private async Task<IResult<string>> CreateRoleAtmAsync(DiscordGuild aDiscordGuild, string aRoleName, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                   .Bind(_ => CreateRoleAtmAsync(aDiscordGuild,aRoleName));

        private async Task<IResult<Unit>> DeleteRoleAtmAsync(DiscordGuild aDiscordGuild, ulong aRoleId, CancellationToken aCancellationToken = default)
        {
            return await Result.CancellationTokenResultAsync(aCancellationToken)
                  .Tap(_ => aDiscordGuild.Roles.FirstOrDefault(r => r.Key == aRoleId).Value.DeleteAsync());

        }

        private async Task<IResult<Unit>> GrantRoleToMemberAtmAsync(DiscordMember aDiscordMember, DiscordRole aDiscordRole, CancellationToken aCancellationToken = default)
        {
            return await Result.CancellationTokenResultAsync(aCancellationToken)
                .Tap(_ => aDiscordMember.GrantRoleAsync(aDiscordRole));

        }

        private async Task<IResult<Unit>> GrantRoleToMemberListAtmAsync(IEnumerable<DiscordMember> aDiscordMemberList, DiscordRole aDiscordRole, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                            .Tap(_ => aDiscordMemberList.ParallelForEachAsync(
                                        _maxDegreeOfParallelism,
                                        lMember => lMember.GrantRoleAsync(aDiscordRole),
                                        aCancellationToken));

        private async Task<IResult<Unit>> RevokeRoleToMemberListAtmAsync(IEnumerable<DiscordMember> aDiscordMemberList, DiscordRole aDiscordRole, CancellationToken aCancellationToken = default)
                        => await Result.CancellationTokenResultAsync(aCancellationToken)
                            .Tap(_ => aDiscordMemberList.ParallelForEachAsync(
                                        _maxDegreeOfParallelism,
                                        lMember => lMember.RevokeRoleAsync(aDiscordRole),
                                        aCancellationToken));

        #endregion

        #region User atomics
        private async Task<IResult<DiscordUser>> GetUserAsync(ulong aUserId)
        {
            var lUser = await Client.GetUserAsync(aUserId);
            return lUser != null
                    ? Result.Success(lUser)
                    : Result.Failure<DiscordUser>(DiscordBotErrors.User.NotFoundId);

        }
        private async Task<IResult<DiscordUser>> GetUserAsync(ulong aUserId, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Bind(_ => GetUserAsync(aUserId));
        #endregion

        #region Member atomics
        private async Task<IResult<DiscordMember>> GetDiscordMemberAtmAsync(DiscordGuild aDiscordGuild, string aFullDiscordHandle)
        {
            var lDiscordHandleParts = aFullDiscordHandle.Split('#');
            var lAllMemberList = await aDiscordGuild.GetAllMembersAsync();
            var lMember = lAllMemberList?.FirstOrDefault(x => x.Username == lDiscordHandleParts[0] && x.Discriminator == lDiscordHandleParts[1]);
            return lMember != null
                ? Result.Success(lMember)
                : Result.Failure<DiscordMember>(DiscordBotErrors.Member.NotFoundHandle);

        }

        private async Task<IResult<DiscordMember>> GetDiscordMemberAtmAsync(DiscordGuild aDiscordGuild, string aFullDiscordHandle, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                   .Bind(_ => GetDiscordMemberAtmAsync(aDiscordGuild, aFullDiscordHandle));


        private static async Task<IResult<ImmutableArray<DiscordMember>>> GetFilteredDiscordMemberListAtm(ImmutableArray<DiscordMember> aAllMemberList, IEnumerable<string[]> aDiscordHandleParts)
        {
            return await Task.Run(() =>
            {
                var lSelectedMemberList = aDiscordHandleParts
              .Select(
                x => aAllMemberList
                     .FirstOrDefault(y => y != null && y.Username == x[0] && y.Discriminator == x[1])
                     ).ToList();

                if (lSelectedMemberList.Any(x => x == null))
                    return Result.Failure<ImmutableArray<DiscordMember>>(DiscordBotErrors.Member.OneNotFoundHandle);

                return Result.SuccessHttp(lSelectedMemberList.ToImmutableArray());
            });
        }
        private async Task<IResult<ImmutableArray<DiscordMember>>> GetDiscordMemberListAtmAsync(DiscordGuild aDiscordGuild, string[] aMemberFullHandleList, CancellationToken aCancellationToken = default)
        {
            IEnumerable<string[]> lDiscordHandleParts = default;
            return await Result.CancellationTokenResultAsync(aCancellationToken)
                .Tap(_ => lDiscordHandleParts = aMemberFullHandleList
                                                .Union(aMemberFullHandleList)
                                                .Select(x => x.Split('#')))
                .Bind(_ => GetAllDiscordMemberListAtmAsync(aDiscordGuild, aCancellationToken))
                .Bind(allMemberList => GetFilteredDiscordMemberListAtm(allMemberList, lDiscordHandleParts));

        }

        private async Task<IResult<ImmutableArray<DiscordMember>>> GetAllDiscordMemberListAtmAsync(DiscordGuild aDiscordGuild, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGuild.GetAllMembersAsync())
                    .Map(discordMemberList => discordMemberList.ToImmutableArray());



        #endregion


        #region Channel atomics
        private async Task<IResult<string>> CreateTemplateChannelsAtmAsync(DiscordGuild aDiscordGuild, DiscordRole aDiscordEveryoneRole, CategoryChannelTemplate aCategoryChannelTemplate, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            var lMakePrivateDiscordOverwriteBuilder = new DiscordOverwriteBuilder[] { new DiscordOverwriteBuilder(aDiscordEveryoneRole).Deny(Permissions.AccessChannels) };
            var lNewCategory = await aDiscordGuild.CreateChannelCategoryAsync(aCategoryChannelTemplate.Name, lMakePrivateDiscordOverwriteBuilder);

            await aCategoryChannelTemplate.ChannelList.ParallelForEachAsync(
                _maxDegreeOfParallelism,
                x => aDiscordGuild.CreateChannelAsync(x.Name, x.ChannelType, position: x.Position, parent: lNewCategory, overwrites: lMakePrivateDiscordOverwriteBuilder),
                aCancellationToken);

            return Result.SuccessHttp(lNewCategory.Id.ToString());

        }

        private async Task<IResult<DiscordChannel>> GetCategoryIdFromName(DiscordGuild aDiscordGiildId, string aDiscordCategoryName, CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();

            var lExistingDiscordChannel = (await aDiscordGiildId.GetChannelsAsync())
                                            .FirstOrDefault(channel => channel.IsCategory
                                                                       && channel.Name == aDiscordCategoryName);
            return lExistingDiscordChannel != null
                    ? Result.SuccessHttp(lExistingDiscordChannel)
                    : Result.Failure<DiscordChannel>(DiscordBotErrors.Channel.NotFoundName);

        }
        #endregion


    }
}
