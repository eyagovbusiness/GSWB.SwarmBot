using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilBot.Handelers
{
    internal static class MembersHandler
    {

        #region Member atomics

        #region Validators


        internal static bool ValidateMemberHandleString(string aFullDiscordHandle)
            => !string.IsNullOrWhiteSpace(aFullDiscordHandle)
                && Regex.IsMatch(aFullDiscordHandle, @"^[a-zA-Z0-9]{2,32}#\d{4}$");


        internal static IHttpResult<Unit> ValidateMemberHandle(string aFullDiscordHandle)
            => ValidateMemberHandleString(aFullDiscordHandle)
                ? Result.SuccessHttp(Unit.Value)
                : Result.Failure<Unit>(DiscordBotErrors.Member.InvalidHandle);

        internal static IHttpResult<Unit> ValidateMemberHandleList(string[] aFullDiscordHandleList)
            => aFullDiscordHandleList
               .All(handle => ValidateMemberHandleString(handle))
                    ? Result.SuccessHttp(Unit.Value)
                    : Result.Failure<Unit>(DiscordBotErrors.Member.InvalidHandle);

        #endregion

        internal static async Task<DiscordMember> GetDiscordMemberAtmAsync(DiscordGuild aDiscordGuild, string aFullDiscordHandle)
        {
            var lDiscordHandleParts = aFullDiscordHandle.Split('#');
            var lAllMemberList = await aDiscordGuild.GetAllMembersAsync();
            return lAllMemberList?.FirstOrDefault(x => x.Username == lDiscordHandleParts[0] && x.Discriminator == lDiscordHandleParts[1]);
        }

        internal static async Task<IHttpResult<DiscordMember>> GetDiscordMemberAtmAsync(DiscordGuild aDiscordGuild, string aFullDiscordHandle, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => GetDiscordMemberAtmAsync(aDiscordGuild, aFullDiscordHandle))
                    .Verify(discordMember => discordMember != null, DiscordBotErrors.Member.NotFoundHandle);

        internal static async Task<IHttpResult<ImmutableArray<DiscordMember>>> GetDiscordMemberListAtmAsync(DiscordGuild aDiscordGuild, string[] aMemberFullHandleList, CancellationToken aCancellationToken = default)
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

        internal static async Task<IHttpResult<IEnumerable<DiscordMember>>> GetDiscordMemberList(DiscordGuild aDiscordGuild, Func<DiscordMember,bool> aFilterFunc, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                           .Bind(_ => GetAllDiscordMemberListAtmAsync(aDiscordGuild, aCancellationToken))
                           .Map(allMemberList => allMemberList.Where(member => aFilterFunc(member)));     

        internal static async Task<IHttpResult<ImmutableArray<DiscordMember>>> GetAllDiscordMemberListAtmAsync(DiscordGuild aDiscordGuild, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGuild.GetAllMembersAsync())
                    .Map(discordMemberList => discordMemberList.ToImmutableArray());

        internal static async Task UpdateBuilderOverwrites(List<DiscordOverwriteBuilder> aDiscordOverwriteBuilderList, DiscordOverwrite aDiscordOverwrite, CancellationToken aCancellationToken = default)
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
