using DSharpPlus;
using DSharpPlus.Entities;
using SwarmBot.Application;
using SwarmBot.Handelers;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;
using static SwarmBot.DiscordBotErrors;

namespace SwarmBot.Handlers
{
    internal class UsersHandler(ISwarmBotDiscordBot aSwarmBotDiscordBot)
    {

        private readonly DiscordClient _client = (aSwarmBotDiscordBot as SwarmBotDiscordBot).Client;
        private readonly GuildsHandler _guildsHandler = new(aSwarmBotDiscordBot);

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

        internal async Task<IHttpResult<IEnumerable<DiscordGuild>>> GetUserGuildListAsync(ulong userId, CancellationToken cancellationToken = default)
            => await Result.CancellationTokenResultAsync(cancellationToken)
            .Bind(_ => _guildsHandler.GetUserGuildListAsync(userId, cancellationToken));


    }
}
