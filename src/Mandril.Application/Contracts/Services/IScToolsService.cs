using Mandril.Domain.ValueObjects;
using TGF.Common.ROP.HttpResult;

namespace Mandril.Application
{
    public interface IScToolsService
    {
        public Task<IHttpResult<IEnumerable<ScShip>>> GetRsiShipList();
    }
}
