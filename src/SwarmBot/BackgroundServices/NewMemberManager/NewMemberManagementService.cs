using DSharpPlus.Entities;
using SwarmBot.Application;
using SwarmBot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SwarmBot.BackgroundServices.NewMemberManager
{
    /// <summary>
    /// New members management service that assign a given role to all new members that has no permissions to upload media in the server and after the specified time replaces it by a normal base rol allowing those members to finally post media in the server.
    /// </summary>
    /// <remarks>Depends on <see cref="ISwarmBotDiscordBot"/>, <see cref="ISwarmBotMembersService"/> and <see cref="ISwarmBotRolesService"/></remarks>
    public class NewMemberManagementService : INewMemberManagementService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly BotNewMembersManagerConfig _botNewMembersManagerConfig;

        public NewMemberManagementService(IServiceScopeFactory aServiceScopeFactory, IConfiguration aConfiguration)
        {
            _serviceScopeFactory = aServiceScopeFactory;

            var lBotNewMembersManagerConfig = new BotNewMembersManagerConfig();
            aConfiguration.Bind("BotNewMembersManager", lBotNewMembersManagerConfig);
            _botNewMembersManagerConfig = lBotNewMembersManagerConfig;
        }

        #region INewMemberManagerService

        /// <summary>
        /// Executes the daily checking of every member with the <see cref="BotNewMembersManagerConfig.NoMediaRoleId"/> 
        /// and replaces the role by <see cref="BotNewMembersManagerConfig.MediaRoleId"/> if it proceeds according with the application settings(<see cref="BotNewMembersManagerConfig.NoMediaDays"/>).
        /// </summary>
        /// <param name="aStoppingToken"></param>
        /// <returns>awaitable <see cref="Task"/>.</returns>
        public async Task DoDailyTaskAsync(CancellationToken aStoppingToken)
        {
            IEnumerable<DiscordMember> lDiscordMemberList = await GetNewDiscordMemberList(CheckMemberJoinedAt, aStoppingToken);
            //If there is any member ready to upgrade from the NoMediaRole to the MediaRole, then replace the role.
            if (lDiscordMemberList.Any())
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var lDiscordRolesControllerService = scope.ServiceProvider.GetRequiredService<ISwarmBotRolesService>();
                await lDiscordRolesControllerService.RevokeRoleToMemberList(_botNewMembersManagerConfig.NoMediaRoleId, lDiscordMemberList, aStoppingToken);
                await lDiscordRolesControllerService.AssignRoleToMemberList(_botNewMembersManagerConfig.MediaRoleId, lDiscordMemberList, aStoppingToken);
            }
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<DiscordMember>> GetNewDiscordMemberList(Func<DiscordMember, bool> aNewMemberFilterFunc, CancellationToken aStoppingToken = default)
        {
            IEnumerable<DiscordMember> lDiscordMemberList = default;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var lDiscordGuildControllerService = scope.ServiceProvider.GetRequiredService<ISwarmBotMembersService>();
                var lDiscordMemberListResult = await lDiscordGuildControllerService.GetMemberList(
                    member => member.Roles.Any(role =>
                        role.Id == _botNewMembersManagerConfig.NoMediaRoleId)
                        && aNewMemberFilterFunc(member)
                    , aStoppingToken);
                if (!lDiscordMemberListResult.IsSuccess)
                    throw new Exception($"Error getting the new members list from {nameof(NewMemberManagementService)}: {lDiscordMemberListResult}");
                lDiscordMemberList = lDiscordMemberListResult.Value;
            }
            return lDiscordMemberList;
        }

        public int GetNoMediaDays()
            => _botNewMembersManagerConfig.NoMediaDays;

        #endregion

        private bool CheckMemberJoinedAt(DiscordMember aDiscordGuildMember)
            => (DateTime.Now - aDiscordGuildMember.JoinedAt).TotalDays > _botNewMembersManagerConfig.NoMediaDays;
    }
}
