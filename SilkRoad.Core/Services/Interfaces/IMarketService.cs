using SilkRoad.Core.Data;
using SilkRoad.Core.Domain;

namespace SilkRoad.Core.Services.Interfaces
{
    public interface IMarketService
    {
        Task<Route.MarketPrices> GetMarketPricesAsync(long gameInstanceId);
        Task UpdateMarketPricesAsync(GameInstance gameInstance, Route.MarketPrices marketPrices);
    }
}