﻿using DSharpPlus;
using DSharpPlus.Entities;
using SwarmBot.Application;
using System.Collections.Concurrent;
using TGF.Common.Extensions;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;
using TGF.Common.ROP.HttpResult.RailwaySwitches;

namespace SwarmBot.Handlers
{
    internal class GuildsHandler
    {
        private readonly DiscordClient _client;

        public GuildsHandler(ISwarmBotDiscordBot aSwarmBotDiscordBot)
        {
            var lSwarmBotDiscordBot = aSwarmBotDiscordBot as SwarmBotDiscordBot;
            _client = lSwarmBotDiscordBot?.Client ?? throw new ArgumentNullException(nameof(aSwarmBotDiscordBot));
        }

        public async Task<IHttpResult<DiscordGuild>> GetGuildById(ulong id, bool callAPI, CancellationToken cancellationToken = default)
             => await Result.CancellationTokenResultAsync(cancellationToken)
            .Map(_ => _client.GetGuildAsync(id, callAPI))
            .Verify(discordGuild => discordGuild != null, DiscordBotErrors.Guild.NotFoundId);

        public async Task<IHttpResult<IEnumerable<DiscordGuild>>> GetUserGuildListAsync(ulong userId, CancellationToken cancellationToken = default)
        {
            var guildsWithUser = new ConcurrentBag<DiscordGuild>();

            await _client.Guilds.Values.ParallelForEachAsync(
                SwarmBotDiscordBot._maxDegreeOfParallelism, // Adjust this based on your performance needs
                async guild =>
                {
                    try
                    {
                        var members = await guild.GetAllMembersAsync();
                        if (members.Any(member => member.Id == userId))
                        {
                            guildsWithUser.Add(guild);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching members for guild {guild.Name}: {ex.Message}");
                    }
                },
                aCancellationToken: cancellationToken
            );

            // Return the result as IHttpResult
            return Result.SuccessHttp(guildsWithUser as IEnumerable<DiscordGuild>);
        }

    }
}
