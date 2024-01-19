using DSharpPlus;
using DSharpPlus.Entities;
using SwarmBot.Application;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace SwarmBot.Handlers
{
    internal class UsersHandler
    {

        internal readonly DiscordClient _client;
        public UsersHandler(ISwarmBotDiscordBot aSwarmBotDiscordBot)
        {
            _client = (aSwarmBotDiscordBot as SwarmBotDiscordBot).Client;
        }

        internal async Task<IHttpResult<DiscordUser>> GetUserAsync(ulong aUserId)
        {
            try
            {
                return Result.SuccessHttp(await _client.GetUserAsync(aUserId));
            }
            catch (DSharpPlus.Exceptions.NotFoundException)
            {
                return Result.SuccessHttp<DiscordUser>(null);
            }

        }
        internal async Task<IHttpResult<DiscordUser>> GetUserAsync(ulong aUserId, CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Bind(_ => GetUserAsync(aUserId));

    }
}
