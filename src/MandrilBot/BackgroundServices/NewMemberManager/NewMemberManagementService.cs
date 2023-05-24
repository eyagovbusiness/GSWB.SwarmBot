using DSharpPlus.Entities;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MandrilBot.BackgroundServices.NewMemberManager
{
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
        /// Executes the daily ckecking of every member with the <see cref="BotNewMembersManagerConfig.NoMediaRoleId"/> 
        /// and replaces the role by <see cref="BotNewMembersManagerConfig.MediaRoleId"/> if it proceeds according with the application settings(<see cref="BotNewMembersManagerConfig.NoMediaDays"/>).
        /// </summary>
        /// <param name="aStoppingToken"></param>
        /// <returns>awaitable <see cref="Task"/>.</returns>
        public async Task DoDailyTaskAsync(CancellationToken aStoppingToken)
        {
            IEnumerable<DiscordMember> lDiscordmemberList = await GetNewDiscordMemberList(CheckMemberJoinedAt, aStoppingToken);
            //If there is any member ready to upgrade from the NoMediaRole to the MediaRole, then replace the role.
            if (lDiscordmemberList.Any())
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var lDiscordRolesControllerService = scope.ServiceProvider.GetRequiredService<IRolesController>();
                    await lDiscordRolesControllerService.RevokeRoleToMemberList(_botNewMembersManagerConfig.NoMediaRoleId, lDiscordmemberList, aStoppingToken);
                    await lDiscordRolesControllerService.AssignRoleToMemberList(_botNewMembersManagerConfig.MediaRoleId, lDiscordmemberList, aStoppingToken);
                }
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Get all members with the NoMediaRole ready to be replaced by the MediaRole accordingly with the time they joined the guild and the appsettings.
        /// </summary>
        /// <param name="aStoppingToken"></param>
        /// <returns>List with all members with the NoMediaRole ready to be replaced by the MediaRole accordingly with the time they joined the guild and the appsettings.</returns>
        public async Task<IEnumerable<DiscordMember>> GetNewDiscordMemberList(Func<DiscordMember, bool> aNewMemberFilterFunc, CancellationToken aStoppingToken = default)
        {
            IEnumerable<DiscordMember> lDiscordmemberList = default;
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var lDiscordGuildControllerService = scope.ServiceProvider.GetRequiredService<IMembersController>();
                var lDiscordmemberListResult = await lDiscordGuildControllerService.GetMemberList(
                    member => member.Roles.Any(role =>
                        role.Id == _botNewMembersManagerConfig.NoMediaRoleId)
                        && aNewMemberFilterFunc(member) 
                    , aStoppingToken);
                if (!lDiscordmemberListResult.IsSuccess)
                    throw new Exception($"Error getting the new members list from {nameof(NewMemberManagementService)}: {lDiscordmemberListResult}");
                lDiscordmemberList = lDiscordmemberListResult.Value;
            }
            return lDiscordmemberList;
        }

        /// <summary>
        /// Get the number of no media days applied as a preemptive measure for new members.
        /// </summary>
        /// <returns><see cref="int"/> with the NoMediaDays configured for this service.</returns>
        public int GetNoMediaDays()
            => _botNewMembersManagerConfig.NoMediaDays;


        #endregion

        private bool CheckMemberJoinedAt(DiscordMember aDiscordGuildMember)
            => (DateTime.Now - aDiscordGuildMember.JoinedAt).TotalDays > _botNewMembersManagerConfig.NoMediaDays;
    }
}
