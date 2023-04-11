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
    internal static class RolesHandler
    {

        internal static async Task<IHttpResult<DiscordRole>> GetDiscordRoleAtm(DiscordGuild aDiscordGuild, ulong aRoleId, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGuild.GetRole(aRoleId))
                    .Verify(discordRole => discordRole != null, DiscordBotErrors.Role.NotFoundId);

        internal static async Task<IHttpResult<string>> CreateRoleAtmAsync(DiscordGuild aDiscordGuild, string aRoleName, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGuild.CreateRoleAsync(aRoleName))
                    .Verify(newRole => newRole != null, DiscordBotErrors.Role.RoleNotCreated)
                    .Map(newRole => newRole.Id.ToString());

        internal static async Task<IHttpResult<Unit>> DeleteRoleAtmAsync(DiscordGuild aDiscordGuild, ulong aRoleId, CancellationToken aCancellationToken = default)
        => await Result.CancellationTokenResultAsync(aCancellationToken)
                .Tap(_ => aDiscordGuild.Roles.FirstOrDefault(r => r.Key == aRoleId).Value?.DeleteAsync());


        internal static async Task<IHttpResult<Unit>> GrantRoleToMemberAtmAsync(DiscordMember aDiscordMember, DiscordRole aDiscordRole, string aReason = null, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Tap(_ => aDiscordMember.GrantRoleAsync(aDiscordRole, aReason));

        internal static async Task<IHttpResult<Unit>> GrantRoleToMemberListAtmAsync(IEnumerable<DiscordMember> aDiscordMemberList, DiscordRole aDiscordRole, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                           .Tap(_ => aDiscordMemberList.ParallelForEachAsync(
                                    MandrilDiscordBot._maxDegreeOfParallelism,
                                    lMember => lMember.GrantRoleAsync(aDiscordRole),
                                    aCancellationToken));

        internal static async Task<IHttpResult<Unit>> RevokeRoleToMemberListAtmAsync(IEnumerable<DiscordMember> aDiscordMemberList, DiscordRole aDiscordRole, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
               .Tap(_ => aDiscordMemberList.ParallelForEachAsync(
                        MandrilDiscordBot._maxDegreeOfParallelism,
                        lMember => lMember.RevokeRoleAsync(aDiscordRole),
                        aCancellationToken));

    }
}
