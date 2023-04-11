using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;

namespace MandrilBot.Handlers
{
    internal class UsersHandler
    {

        internal readonly DiscordClient _client;
        public UsersHandler(IMandrilDiscordBot aMandrilDiscordBot)
        {
            _client = (aMandrilDiscordBot as MandrilDiscordBot).Client;
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
