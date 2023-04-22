using DSharpPlus.Entities;
using MandrilBot.Configuration;
using MandrilBot.Controllers;
using MandrilBot.News;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using TGF.Common.Extensions;
using TGF.Common.Extensions.Serialization;
using static System.Formats.Asn1.AsnWriter;

namespace MandrilBot
{
    /// <summary>
    /// BackgroundService to start asynchronously in the background the connection of the internal configured bot with Discord.
    /// </summary>
    public class MandrilDiscordBotBackgroundTasks : BackgroundService
    {
        private readonly IMandrilDiscordBot _mandrilDiscordBotService;
        private readonly IDiscordBotNewsService _discordBotNewsService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger _logger;

        private readonly BotWelcomeConfig _botWelcomeConfig;
        private readonly string _newMembersFilePath = "/etc/NewUsers.json";

        public MandrilDiscordBotBackgroundTasks(
            IMandrilDiscordBot aMandrilDiscordBot,
            IDiscordBotNewsService aDiscordBotNewsService,
            IServiceScopeFactory aServiceScopeFactory,
            IConfiguration aConfiguration,
            ILoggerFactory aLoggerFactory)
        {
            _mandrilDiscordBotService = aMandrilDiscordBot;
            _discordBotNewsService = aDiscordBotNewsService;
            _serviceScopeFactory = aServiceScopeFactory;
            _logger = aLoggerFactory.CreateLogger(typeof(MandrilDiscordBotBackgroundTasks));

            var lNewWelcomeConfig = new BotWelcomeConfig();
            aConfiguration.Bind("BotWelcome", lNewWelcomeConfig);
            _botWelcomeConfig = lNewWelcomeConfig;

        }

        protected override async Task ExecuteAsync(CancellationToken aStoppingToken)
        {
            try
            {
                await _mandrilDiscordBotService.StartAsync();

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var lDiscordChannelsControllerService = scope.ServiceProvider.GetRequiredService<IChannelsController>();
                    await _discordBotNewsService.InitAsync(lDiscordChannelsControllerService);
                }

                MakeSureFileExist(_newMembersFilePath, aStoppingToken);

                while (!aStoppingToken.IsCancellationRequested)
                {
                    if (DateTimeOffset.UtcNow.TimeOfDay == new TimeSpan(5, 0, 0)) //true every day at 5 AM UTC
                        await DoDaylyTaskAsync(aStoppingToken);

                    await Task.Delay(10000, aStoppingToken);
                    await _discordBotNewsService.TickExecute(aStoppingToken);
                }
            }
            catch (Exception lException)
            {
                _logger.LogError("An error occurred during the execution of MandrilDiscordBotBackgroundTasks: ", lException.ToString());
            }

        }

        private async Task DoDaylyTaskAsync(CancellationToken aStoppingToken)
        {
            // Read the JSON file
            var lNewMemberDictionary = await SerializationExtensions.DeserializeFromFileAsync<Dictionary<ulong, DateTimeOffset>>(_newMembersFilePath);

            // Remove entries where the DateTime value has already passed
            var lKeyListToRemove = lNewMemberDictionary.Where(pair => pair.Value < DateTime.Now).Select(pair => pair.Key).ToArray();
            if(lKeyListToRemove.Any())
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var lDiscordRolesControllerService = scope.ServiceProvider.GetRequiredService<IRolesController>();
                    await lDiscordRolesControllerService.RevokeRoleToMemberList(_botWelcomeConfig.NoMediaRoleId, lKeyListToRemove, aStoppingToken);
                    await lDiscordRolesControllerService.AssignRoleToMemberList(_botWelcomeConfig.MediaRoleId, lKeyListToRemove, aStoppingToken);
                }
                foreach (var lKey in lKeyListToRemove)
                    lNewMemberDictionary.Remove(lKey);
            }

            lNewMemberDictionary = await GetUpdatedNewMemberDictionaryAsync(lNewMemberDictionary, aStoppingToken);
            // Write the updated dictionary back to the file
            await SerializationExtensions.SerializeToFileAsync(lNewMemberDictionary, _newMembersFilePath, aOverride: true);

            await Task.CompletedTask;
        }

        private async Task<Dictionary<ulong, DateTimeOffset>> GetUpdatedNewMemberDictionaryAsync(Dictionary<ulong, DateTimeOffset> aSourceNewMemberDictionary, CancellationToken aStoppingToken)
        {
            IEnumerable<DiscordMember> lDiscordmemberList;
            //Get all members with the NoMediaRole
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var lDiscordGuildControllerService = scope.ServiceProvider.GetRequiredService<IMembersController>();
                var lDiscordmemberListResult = await lDiscordGuildControllerService.GetMemberList(member => member.Roles.Any(role => role.Id == _botWelcomeConfig.NoMediaRoleId), aStoppingToken);
                if (!lDiscordmemberListResult.IsSuccess)
                    throw new Exception($"Error getting the MembersList with NoMedia role: {lDiscordmemberListResult}");
                lDiscordmemberList = lDiscordmemberListResult.Value;
            }
            //Get from the members with the NoMediaRole the ones that were not yet added to the storage file.
            IEnumerable<DiscordMember> lMemberMissingList = lDiscordmemberList.Where(member => !aSourceNewMemberDictionary.ContainsKey(member.Id));
            //Update the Dictionary with the missing member in file, calculating the date when the NoMedia role has to replaced by the MediaRole.
            lMemberMissingList.ForEach(
                missingMember => 
                {
                    var lMultiplier = (DateTime.Now - missingMember.CreationTimestamp).Days < 7 ? _botWelcomeConfig.NoMediaDaysMultiplier : 1;
                    aSourceNewMemberDictionary.Add(missingMember.Id, missingMember.JoinedAt.AddDays(_botWelcomeConfig.BaseNoMediaDays * lMultiplier));
                });

            return aSourceNewMemberDictionary;
        }

        private static void MakeSureFileExist(string aFilePath, CancellationToken aStoppingToken)
        {
            using StreamWriter lNewJsomFile = File.CreateText(aFilePath);
            lNewJsomFile.Write("{}");
            lNewJsomFile.Close();

        }
    }
}
