using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SilkRoad.Core.Data;
using SilkRoad.Core.Domain;
using SilkRoad.Core.Services.Interfaces;

namespace SilkRoad.Core.Services
{
    public class MarketService : IMarketService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<MarketService> _logger;

        public MarketService(GameDbContext context, ILogger<MarketService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Route.MarketPrices> GetMarketPricesAsync(long gameInstanceId)
        {
            _logger.LogDebug("Loading market prices for GameInstance {GameInstanceId}", gameInstanceId);
            var marketState = await _context.GameStates
                .FirstOrDefaultAsync(gs => gs.GameInstanceId == gameInstanceId && gs.StateKey == "MarketPrices");
            return marketState != null ? JsonSerializer.Deserialize<Route.MarketPrices>(marketState.StateValue) : new Route.MarketPrices();
        }

        public async Task UpdateMarketPricesAsync(GameInstance gameInstance, Route.MarketPrices marketPrices)
        {
            _logger.LogDebug("Updating market prices for GameInstance {GameInstanceId}", gameInstance.Id);

            var marketGroups = gameInstance.EntityRelationships
                .Where(r => r.RelationshipType == "Trade")
                .GroupBy(r => r.TargetEntityId)
                .ToList();

            foreach (var group in marketGroups)
            {
                var market = gameInstance.Entities.FirstOrDefault(e => e.Id == group.Key);
                var marketAttr = market?.EntityAttributes.FirstOrDefault(a => a.AttributeKey == "Demand");
                if (marketAttr == null)
                {
                    _logger.LogWarning("Market ID {MarketId} has no demand attribute.", group.Key);
                    continue;
                }

                var demand = JsonSerializer.Deserialize<Route.MarketDemand>(marketAttr.AttributeValue) ?? new Route.MarketDemand();
                if (demand.GoodsDemand == null || !demand.GoodsDemand.Any())
                {
                    demand.GoodsDemand = new Dictionary<string, Route.GoodDemand>
                    {
                        { "Silk", new Route.GoodDemand { Price = market.Name == "ChangAnMarket" ? 12.0 : 30.0, MaxQuantity = market.Name == "ChangAnMarket" ? 100 : 80 } },
                        { "Spices", new Route.GoodDemand { Price = market.Name == "ChangAnMarket" ? 8.0 : 15.0, MaxQuantity = market.Name == "ChangAnMarket" ? 50 : 40 } }
                    };
                    _logger.LogWarning("No demand data for market {MarketName}. Using default demand.", market.Name);
                }

                int caravanCount = group.Count();
                foreach (var good in demand.GoodsDemand)
                {
                    double originalPrice = good.Value.Price;
                    good.Value.Price *= caravanCount > 1 ? 0.8 : 1.0;
                    _logger.LogDebug("Market {MarketName}: Adjusted price for {GoodType} from {OriginalPrice} to {NewPrice} due to {CaravanCount} caravans.",
                        market.Name, good.Key, originalPrice, good.Value.Price, caravanCount);
                }
                marketAttr.AttributeValue = JsonSerializer.Serialize(demand);
                marketAttr.UpdatedAt = DateTime.UtcNow;
            }

            var marketState = gameInstance.GameStates.FirstOrDefault(gs => gs.StateKey == "MarketPrices");
            if (marketState != null)
            {
                marketState.StateValue = JsonSerializer.Serialize(marketPrices);
                marketState.UpdatedAt = DateTime.UtcNow;
                _logger.LogDebug("Updated MarketPrices in GameState for GameInstance {GameInstanceId}.", gameInstance.Id);
            }
        }
    }
}