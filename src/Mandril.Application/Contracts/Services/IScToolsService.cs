using Mandril.Domain.ValueObjects;
using TGF.Common.ROP.HttpResult;

namespace Mandril.Application
{
    public interface IScToolsService
    {
        public Task<IHttpResult<List<Ship>>> GetRsiShipList();
        public Task GetRsiData();
    }
}
