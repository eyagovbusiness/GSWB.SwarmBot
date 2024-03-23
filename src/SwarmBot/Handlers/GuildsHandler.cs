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
        private readonly ulong _testersGuildId;
        public GuildsHandler(ISwarmBotDiscordBot aSwarmBotDiscordBot)
        {
            var lSwarmBotDiscordBot = aSwarmBotDiscordBot as SwarmBotDiscordBot;
            _client = lSwarmBotDiscordBot.Client;
            _guildId = lSwarmBotDiscordBot.BotConfiguration.DiscordTargetGuildId;
            _testersGuildId = lSwarmBotDiscordBot.BotConfiguration.TestersDiscordTargetGuildId;
        }
        public async Task<IHttpResult<DiscordGuild>> GetDiscordGuildFromConfigAsync(CancellationToken aCancellationToken = default, bool? aCallAPI = null)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
            .Map(_ => _client.GetGuildAsync(_guildId, aCallAPI))
            .Verify(discordGuild => discordGuild != null, DiscordBotErrors.Guild.NotFoundId);
        public async Task<IHttpResult<DiscordGuild>> GetTestersDiscordGuildFromConfigAsync(CancellationToken aCancellationToken = default, bool? aCallAPI = null)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
            .Map(_ => _client.GetGuildAsync(_testersGuildId, aCallAPI))
            .Verify(discordGuild => discordGuild != null, DiscordBotErrors.Guild.NotFoundId);
    }
}
