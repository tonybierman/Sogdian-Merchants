using SilkRoad.Core.Data;
using SilkRoad.Core.Domain;

namespace SilkRoad.Core.Services.Interfaces
{
    public interface ICaravanProcessor
    {
        Task ProcessCaravanAsync(Entity caravan, GameInstance gameInstance, Route.MarketPrices marketPrices, Route.RandomEvents randomEvents);
    }
}