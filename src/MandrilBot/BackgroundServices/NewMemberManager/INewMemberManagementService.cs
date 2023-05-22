using DSharpPlus.Entities;
using MandrilBot.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandrilBot.BackgroundServices.NewMemberManager
{
    public interface INewMemberManagementService
    {
        /// <summary>
        /// Executes the daily ckecking of every member with the <see cref="BotNewMembersManagerConfig.NoMediaRoleId"/> 
        /// and replaces the role by <see cref="BotNewMembersManagerConfig.MediaRoleId"/> if it proceeds according with the application settings(<see cref="BotNewMembersManagerConfig.BaseNoMediaDays"/>).
        /// </summary>
        /// <param name="aStoppingToken"></param>
        /// <returns>awaitable <see cref="Task"/>.</returns>
        public Task DoDailyTaskAsync(CancellationToken aStoppingToken);

        /// <summary>
        /// Get all members with the NoMediaRole ready to be replaced by the MediaRole accordingly with the time they joined the guild and the appsettings.
        /// </summary>
        /// <param name="aStoppingToken"></param>
        /// <returns>List with all members with the NoMediaRole ready to be replaced by the MediaRole accordingly with the time they joined the guild and the appsettings.</returns>
        public Task<IEnumerable<DiscordMember>> GetNewDiscordMemberList(Func<DiscordMember, bool> aNewMemberFilterFunc, CancellationToken aStoppingToken = default);

    }
}
