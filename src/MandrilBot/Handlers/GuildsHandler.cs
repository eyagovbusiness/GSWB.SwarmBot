using DSharpPlus;
using DSharpPlus.Entities;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilBot.Handelers
{
    internal class GuildsHandler
    {
        private readonly DiscordClient _client;
        private readonly ulong _guildId;
        internal GuildsHandler(IMandrilDiscordBot aMandrilDiscordBot) 
        {
            var lMandrilDiscordBot = aMandrilDiscordBot as MandrilDiscordBot;
            _client = lMandrilDiscordBot.Client;
            _guildId= lMandrilDiscordBot._botConfiguration.DiscordTargetGuildId;
        }
        internal async Task<IHttpResult<DiscordGuild>> GetDiscordGuildFromConfigAsync(CancellationToken aCancellationToken = default)
            => await Result.CancellationTokenResultAsync(aCancellationToken)
                    .Map(_ => _client.GetGuildAsync(_guildId))
                    .Verify(discordGuild => discordGuild != null, DiscordBotErrors.Guild.NotFoundId);
    }
}
