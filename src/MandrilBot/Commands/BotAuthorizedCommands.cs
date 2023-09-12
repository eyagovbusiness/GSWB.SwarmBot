using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Mandril.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MandrilBot.Commands
{
    internal abstract class BotAuthorizedCommands : BaseCommandModule
    {
        protected readonly Lazy<Task<int>> _authorizeCommandRolePosition;
        protected readonly IServiceScopeFactory _serviceScopeFactory;

        internal BotAuthorizedCommands(IServiceScopeFactory aServiceScopeFactory, IConfiguration aConfiguration, string aConfigKeyName)
        {
            _serviceScopeFactory = aServiceScopeFactory;    
            if(string.IsNullOrEmpty(aConfigKeyName))
                throw new ArgumentNullException("Error, when crating a new instance of BotAuthorizedCommands derived classes, the argument aConfigKeyName must be provided with a valid value.");
            var lAuthorizeRoleId = aConfiguration.GetValue<ulong>(aConfigKeyName);
            _authorizeCommandRolePosition = new Lazy<Task<int>>(GetAuthorizeCommandRole(lAuthorizeRoleId));
        }

        private async Task<int> GetAuthorizeCommandRole(ulong aAuthorizeRoleId)
        {
            using var lScope = _serviceScopeFactory.CreateScope();

            var lMandrilRolesService = lScope.ServiceProvider.GetRequiredService<IMandrilRolesService>();
            var lRoleList = await lMandrilRolesService.GetGuildServerRoleList();

            if (!lRoleList.IsSuccess)
                throw new Exception("Error while lazy initializing value for '_authorizeCommandRole' in a derived class of BotAuthorizedCommands. Failure on fetching guild role list.");

            var lAuthorizeCommandRole = lRoleList.Value.FirstOrDefault(role => role.Id == aAuthorizeRoleId) 
                ?? throw new Exception("Error while lazy initializing value for '_authorizeCommandRole' in a derived class of BotAuthorizedCommands. No guild roleId matches the required roleId if to authorize this commands.");
            return lAuthorizeCommandRole.Position;

        }

        protected async Task<bool> IsMemberAuthorized(DiscordMember aDiscordMember)
        {
            var lMinRolePositionRequired = await _authorizeCommandRolePosition.Value;
            return aDiscordMember.Roles.Any(x => x.Position >= lMinRolePositionRequired);
        }
    }
}
