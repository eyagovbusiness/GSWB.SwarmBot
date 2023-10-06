using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Mandril.Application;
using MandrilBot.BackgroundServices.NewMemberManager;
using MandrilBot.Commands;
using MandrilBot.Configuration;
using MandrilBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using TGF.CA.Application;
using TGF.CA.Infrastructure.Communication.Publisher.Integration;

namespace MandrilBot
{
    /// <summary>
    /// Class designed as a service to provide communication with an internal Discord bot in this service, with functionality accessible through the public interface.
    /// </summary>
    public partial class MandrilDiscordBot : IMandrilDiscordBot
    {
        public static readonly byte _maxDegreeOfParallelism = Convert.ToByte(Math.Ceiling(Environment.ProcessorCount * 0.75));
        private bool mIsAutoBanBotsEnabled = true;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        private readonly ISecretsManager _secretsManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        internal BotConfig BotConfiguration { get; private set; }

        internal DiscordClient Client { get; private set; }
        internal CommandsNextExtension Commands { get; private set; }

        public MandrilDiscordBot(ILoggerFactory aLoggerFactory, ISecretsManager aSecretsManager, IConfiguration aConfiguration, IServiceScopeFactory aServiceScopeFactory)
        {
            _loggerFactory = aLoggerFactory;
            _secretsManager = aSecretsManager;
            _configuration = aConfiguration;
            _serviceScopeFactory = aServiceScopeFactory;
        }

        #region IMandrilDiscordBot

        public async Task<HealthCheckResult> GetHealthCheck(CancellationToken aCancellationToken = default)
        {
            aCancellationToken.ThrowIfCancellationRequested();
            try
            {
                var lGuilPreview = await Client.GetGuildPreviewAsync(BotConfiguration.DiscordTargetGuildId);
                if (lGuilPreview?.ApproximateMemberCount != null && lGuilPreview?.ApproximateMemberCount > 0)
                {
                    if (Client.Ping < 300)
                        return HealthCheckResult.Healthy(string.Format("The service is healthy. ({0}ms) ping", Client.Ping));
                    else
                        return HealthCheckResult.Degraded(string.Format("The service's health is degraded due to high latency. ({0}ms) ping", Client.Ping));
                }
            }
            catch (Exception lException)
            {
                return HealthCheckResult.Unhealthy("The service is down at the moment, an exception was thrown fetching the guild preview.", lException);
            }
            return HealthCheckResult.Unhealthy("The service is down at th moment, could not fetch guild preview.");

        }

        public async Task StartAsync()
        {
            try
            {
                await SetBotConfigurationAsync();

                ConfigureDiscordBotClient();
                ConfigureDiscordBotCommands();

                await Client.ConnectAsync();
            }
            catch (Exception lException)
            {
                _loggerFactory.CreateLogger(typeof(MandrilDiscordBot)).LogError("An error occurred while attempting to start the Discord bot client: {0}. Stack trace: {1} ", lException.ToString(), lException.StackTrace);
            }
        }

        public async Task<bool> AllowTemporarilyJoinNewBots(int aMinutesAllowed)
        {
            if (!mIsAutoBanBotsEnabled)
                return mIsAutoBanBotsEnabled;
            mIsAutoBanBotsEnabled = !mIsAutoBanBotsEnabled;
            await Task.Delay(TimeSpan.FromMinutes(aMinutesAllowed));
            mIsAutoBanBotsEnabled = !mIsAutoBanBotsEnabled;
            return mIsAutoBanBotsEnabled;
        }


        #endregion

        #region BotConfiguration

        private void ConfigureDiscordBotClient()
        {
            var config = new DiscordConfiguration
            {
                UseRelativeRatelimit = true,
                LoggerFactory = _loggerFactory,
                Token = BotConfiguration.MandrilBotToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                Intents = DiscordIntents.MessageContents
                          | DiscordIntents.GuildMessageReactions
                          | DiscordIntents.GuildMessages
                          | DiscordIntents.Guilds
                          | DiscordIntents.GuildVoiceStates
                          | DiscordIntents.GuildPresences
                          | DiscordIntents.GuildMembers
            };

            Client = new DiscordClient(config);
            //By default the bot will be banning new bots unless it is temporary disabled via admin commands.
            Client.GuildMemberAdded += OnGuildMemberAdded;
            Client.GuildMemberUpdated += OnGuildMemberUpdated;
            Client.GuildRoleCreated += OnGuildRoleCreated;
            Client.GuildRoleDeleted += OnGuildRoleDeleted;
            Client.GuildRoleUpdated += OnGuildRoleUpdated;
        }

        private Task OnGuildRoleUpdated(DiscordClient sender, GuildRoleUpdateEventArgs args)
        {
            //create role updated dto for message with changes about role name or position and include roleId and guildId
            //args.RoleAfter.Id = null;
            //if(args.RoleBefore.Name != args.RoleAfter.Name)
            //    args.RoleAfter.Name = null;
            //if (args.RoleBefore.Position != args.RoleAfter.Position)
            //    args.RoleAfter.Position = null;
            //args.Guild.Id = null;
            return Task.CompletedTask;
        }

        private Task OnGuildRoleDeleted(DiscordClient sender, GuildRoleDeleteEventArgs args)
        {
            //create role deleted dto for message with roleId and guildId
            //args.Role.Id = null;
            //args.Guild.Id = null;
            return Task.CompletedTask;
        }

        private Task OnGuildRoleCreated(DiscordClient sender, GuildRoleCreateEventArgs args)
        {
            //create role created dto for message with role id, name, position and include guildId      
            //args.Role.Id = null;
            //args.Role.Name = null;
            //args.Role.Position = null;
            //args.Guild.Id = null;
            return Task.CompletedTask;
        }

        private async Task SetBotConfigurationAsync()
        {
            BotConfiguration = await _secretsManager.Get<BotConfig>("mandrilbot");
            BotConfiguration.BotCommandPrefix = _configuration.GetValue<string>("BotCommandPrefix");
        }

        private void ConfigureDiscordBotCommands()
        {
            var lCommandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { BotConfiguration.BotCommandPrefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                Services = GetCommandsServiceProvider(),
            };

            Commands = Client.UseCommandsNext(lCommandsConfig);
            Commands.RegisterCommands<BotAdminCommands>();
            Commands.RegisterCommands<BotTrustedMemberCommands>();
        }

        private IServiceProvider GetCommandsServiceProvider()
        {
            var lNewConfiguration = new ConfigurationBuilder()
                .AddConfiguration(_configuration)
                .Build();

            var lServices = new ServiceCollection();
            lServices.AddSingleton<IMandrilDiscordBot>(this);
            lServices.AddScoped<IMandrilMembersService, MandrilMembersService>();
            lServices.AddScoped<IMandrilRolesService, MandrilRolesService>();
            lServices.AddScoped<INewMemberManagementService, NewMemberManagementService>();
            lServices.AddSingleton<IConfiguration>(lNewConfiguration); // Register lNewConfiguration as IConfiguration
            return lServices.BuildServiceProvider();
        }

        #endregion

        #region BotEvents

        /// <summary>
        /// Ban every new bot that joins the server unless it is allowed for short time by an admin using the admin command "?open-bots".
        /// </summary>
        private async Task OnGuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            if (mIsAutoBanBotsEnabled && e.Guild.Id == BotConfiguration.DiscordTargetGuildId && e.Member.IsBot)
            {
                await e.Member.BanAsync(reason: "Bot detected.");
                Console.WriteLine($"Banned bot: {e.Member.Username}#{e.Member.Discriminator}.");
            }
        }

        private Task OnGuildMemberUpdated(DiscordClient sender, GuildMemberUpdateEventArgs e)
        {
            var lAddedRoles = e.RolesAfter.Except(e.RolesBefore).ToList();
            var lRemovedRoles = e.RolesBefore.Except(e.RolesAfter).ToList();

            if (lAddedRoles.Any())
            {
                SendRolesAddedMessage(lAddedRoles);
            }

            if (lRemovedRoles.Any())
            {
                SendRolesRevokedMessage(lRemovedRoles);
                //Console.WriteLine($"{e.Member.Username} lost roles: {string.Join(", ", lRemovedRoles.Select(r => r.Name))}");
            }

            return Task.CompletedTask;
        }

        #endregion

        private void SendRolesAddedMessage(List<DiscordRole> aAddedRoleList)
        {
            //TO-DO:create dto with list fo roles, user id and a bool to indicate if the property is Removed or Added.Create an exchange that send the message to one queue to the members microservice and then the members send the message to api gateway when a member roles are updated.
            //This is good because we centralize in the member microservice revocation(when an applicationn roles change permissiosn and when a member addd/remove a role)
            //using var scope = _serviceScopeFactory.CreateScope();
            //var lIntegrationMessagePublisher = scope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>();
            //lIntegrationMessagePublisher.Publish(aAddedRoleList, routingKey: "member.roles.update");
        }

        private void SendRolesRevokedMessage(List<DiscordRole> aAddedRoleList)
        {
            //TO-DO:create dto with list fo roles, user id and a bool to indicate if the property is Removed or Added.Create an exchange that send the message to one queue to the members microservice and then the members send the message to api gateway when a member roles are updated.
            //This is good because we centralize in the member microservice revocation(when an applicationn roles change permissiosn and when a member addd/remove a role)
            //using var scope = _serviceScopeFactory.CreateScope();
            //var lIntegrationMessagePublisher = scope.ServiceProvider.GetRequiredService<IIntegrationMessagePublisher>();
            //lIntegrationMessagePublisher.Publish(aAddedRoleList, routingKey: "member.roles.update");
        }

    }
}