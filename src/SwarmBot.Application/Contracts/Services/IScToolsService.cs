using SwarmBot.Domain.ValueObjects;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;

namespace SwarmBot.Application
{
    public interface IScToolsService
    {
        public Task<IHttpResult<List<Ship>>> GetRsiShipList();
        public Task<IHttpResult<Unit>> GetRsiData();
    }
}
