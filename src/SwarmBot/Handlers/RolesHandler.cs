using DSharpPlus.Entities;
using TGF.Common.Extensions;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace SwarmBot.Handlers
{
    public static class RolesHandler
    {

        public static async Task<IHttpResult<DiscordRole>> GetDiscordRoleAtm(DiscordGuild aDiscordGuild, ulong aRoleId, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGuild.GetRole(aRoleId))
                    .Verify(discordRole => discordRole != null, DiscordBotErrors.Role.NotFoundId);

        public static async Task<IHttpResult<ulong>> CreateRoleAtmAsync(DiscordGuild aDiscordGuild, string aRoleName, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => aDiscordGuild.CreateRoleAsync(aRoleName))
                    .Verify(newRole => newRole != null, DiscordBotErrors.Role.RoleNotCreated)
                    .Map(newRole => newRole.Id);

        public static async Task<IHttpResult<Unit>> DeleteRoleAtmAsync(DiscordGuild aDiscordGuild, ulong aRoleId, CancellationToken aCancellationToken = default)
        => await Result.CancellationTokenResultAsync(aCancellationToken)
                .Tap(_ => aDiscordGuild.Roles.FirstOrDefault(r => r.Key == aRoleId).Value?.DeleteAsync()!);


        public static async Task<IHttpResult<Unit>> GrantRoleToMemberAtmAsync(DiscordMember aDiscordMember, DiscordRole aDiscordRole, string aReason = null!, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Tap(_ => aDiscordMember.GrantRoleAsync(aDiscordRole, aReason));

        public static async Task<IHttpResult<Unit>> GrantRoleToMemberListAtmAsync(IEnumerable<DiscordMember> aDiscordMemberList, DiscordRole aDiscordRole, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                           .Tap(_ => aDiscordMemberList.ParallelForEachAsync(
                                    SwarmBotDiscordBot._maxDegreeOfParallelism,
                                    lMember => lMember.GrantRoleAsync(aDiscordRole),
                                    aCancellationToken));

        public static async Task<IHttpResult<Unit>> RevokeRoleToMemberListAtmAsync(IEnumerable<DiscordMember> aDiscordMemberList, DiscordRole aDiscordRole, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
               .Tap(_ => aDiscordMemberList.ParallelForEachAsync(
                        SwarmBotDiscordBot._maxDegreeOfParallelism,
                        lMember => lMember.RevokeRoleAsync(aDiscordRole),
                        aCancellationToken));

    }
}
