using DSharpPlus;
using DSharpPlus.Entities;
using SwarmBot.Application;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace SwarmBot.Handelers
{
    internal class GuildsHandler
    {
        private readonly DiscordClient _client;
        private readonly ulong _guildId;
        public GuildsHandler(ISwarmBotDiscordBot aSwarmBotDiscordBot)
        {
            var lSwarmBotDiscordBot = aSwarmBotDiscordBot as SwarmBotDiscordBot;
            _client = lSwarmBotDiscordBot.Client;
            _guildId = lSwarmBotDiscordBot.BotConfiguration.DiscordTargetGuildId;
        }
        public async Task<IHttpResult<DiscordGuild>> GetDiscordGuildFromConfigAsync(CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => _client.GetGuildAsync(_guildId))
                    .Verify(discordGuild => discordGuild != null, DiscordBotErrors.Guild.NotFoundId);
    }
}
