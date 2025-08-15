using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SilkRoad.Core.Data;
using SilkRoad.Core.Domain;
using SilkRoad.Core.Services.Interfaces;

namespace SilkRoad.Core.Services
{
    public class CaravanProcessor : ICaravanProcessor
    {
        private readonly Random _random;
        private readonly ILogger<CaravanProcessor> _logger;
        private readonly GameDbContext _context;

        public CaravanProcessor(GameDbContext context, ILogger<CaravanProcessor> logger)
        {
            _context = context;
            _random = new Random();
            _logger = logger;
        }

        public async Task ProcessCaravanAsync(Entity caravan, GameInstance gameInstance, Route.MarketPrices marketPrices, Route.RandomEvents randomEvents)
        {
            _logger.LogInformation("Processing caravan {CaravanName} (ID: {CaravanId})", caravan.Name, caravan.Id);

            var goodsAttr = caravan.EntityAttributes.FirstOrDefault(a => a.AttributeKey == "Goods");
            var routeAttr = caravan.EntityAttributes.FirstOrDefault(a => a.AttributeKey == "Route");
            var investmentAttr = caravan.EntityAttributes.FirstOrDefault(a => a.AttributeKey == "Investment");
            var statusAttr = caravan.EntityAttributes.FirstOrDefault(a => a.AttributeKey == "Status");

            if (goodsAttr == null || routeAttr == null || investmentAttr == null || statusAttr == null)
            {
                _logger.LogWarning("Caravan {CaravanName} missing required attributes.", caravan.Name);
                return;
            }

            var goods = JsonSerializer.Deserialize<Route.Goods>(goodsAttr.AttributeValue);
            var route = JsonSerializer.Deserialize<Route>(routeAttr.AttributeValue);
            var investment = JsonSerializer.Deserialize<Route.Investment>(investmentAttr.AttributeValue);

            if (goods == null || route == null || investment == null)
            {
                _logger.LogError("Caravan {CaravanName} failed to deserialize attributes.", caravan.Name);
                return;
            }

            _logger.LogDebug("Caravan {CaravanName}: Goods={GoodsType}, Quantity={Quantity}, Route={RouteStart}->{RouteEnd}, Risk={RiskLevel}, Investment={Investment}",
                caravan.Name, goods.Type, goods.Quantity, route.Start, route.End, route.RiskLevel, investment.Value);

            var partnership = gameInstance.EntityRelationships
                .FirstOrDefault(r => r.SourceEntityId == caravan.Id && r.RelationshipType == "Partnership");
            bool playerCooperates = true;
            bool rivalCooperates = _random.NextDouble() < 0.7;

            if (partnership != null)
            {
                _logger.LogInformation("Caravan {CaravanName} has partnership. Player cooperates: {PlayerCooperates}, Rival cooperates: {RivalCooperates}",
                    caravan.Name, playerCooperates, rivalCooperates);
            }

            var tollRelationship = gameInstance.EntityRelationships
                .FirstOrDefault(r => r.SourceEntityId == caravan.Id && r.RelationshipType == "TollNegotiation");
            bool paidToll = tollRelationship != null && _random.NextDouble() < 0.8;

            if (tollRelationship != null)
            {
                _logger.LogInformation("Caravan {CaravanName} negotiated toll. Paid: {PaidToll}", caravan.Name, paidToll);
            }

            double eventProbability = route.RiskLevel switch
            {
                "High" => 0.4,
                "Medium" => 0.2,
                "Low" => 0.1,
                _ => 0.2
            };

            bool eventTriggered = _random.NextDouble() < eventProbability;
            double goodsLoss = 0;
            if (eventTriggered && !paidToll && randomEvents.Events.Any())
            {
                var evt = randomEvents.Events.OrderBy(_ => _random.Next()).First();
                goodsLoss = evt.Value.Impact switch
                {
                    "Lose 50% goods" => 0.5,
                    "Lose 30% goods" => 0.3,
                    _ => 0
                };
                _logger.LogInformation("Caravan {CaravanName} triggered event {EventName} with {GoodsLoss:P0} goods loss.", caravan.Name, evt.Key, goodsLoss);
            }

            var tradeRelationship = gameInstance.EntityRelationships
                .FirstOrDefault(r => r.SourceEntityId == caravan.Id && r.RelationshipType == "Trade");
            if (tradeRelationship == null)
            {
                _logger.LogWarning("Caravan {CaravanName} has no trade relationship.", caravan.Name);
                return;
            }

            var market = gameInstance.Entities.FirstOrDefault(e => e.Id == tradeRelationship.TargetEntityId);
            var marketAttr = market?.EntityAttributes.FirstOrDefault(a => a.AttributeKey == "Demand");
            if (marketAttr == null)
            {
                _logger.LogWarning("Market for caravan {CaravanName} has no demand attribute.", caravan.Name);
                return;
            }

            var demand = JsonSerializer.Deserialize<Route.MarketDemand>(marketAttr.AttributeValue) ?? new Route.MarketDemand();
            double price = 10;
            if (demand.GoodsDemand != null && goods.Type != null && demand.GoodsDemand.TryGetValue(goods.Type, out var goodDemand))
            {
                price = goodDemand.Price;
                _logger.LogDebug("Caravan {CaravanName} selling {GoodsType} at price {Price} in market {MarketName}.",
                    caravan.Name, goods.Type, price, market.Name);
            }
            else
            {
                _logger.LogWarning("No demand data for {GoodsType} in market {MarketName}. Using default price: {DefaultPrice}.",
                    goods.Type, market.Name, price);
            }

            int remainingQuantity = (int)(goods.Quantity * (1 - goodsLoss));
            double basePayoff = remainingQuantity * price;
            double finalPayoff = partnership != null
                ? (playerCooperates && rivalCooperates ? basePayoff * 0.5 : playerCooperates && !rivalCooperates ? 0 : basePayoff)
                : basePayoff;

            _logger.LogInformation("Caravan {CaravanName} payoff: {FinalPayoff}, remaining quantity: {RemainingQuantity}",
                caravan.Name, finalPayoff, remainingQuantity);

            var player = gameInstance.Entities.FirstOrDefault(e => e.EntityType == "Merchant" && e.Name == "Player");
            if (player != null)
            {
                var capitalAttr = player.EntityAttributes.FirstOrDefault(a => a.AttributeKey == "Capital");
                if (capitalAttr != null)
                {
                    var capital = JsonSerializer.Deserialize<Route.Capital>(capitalAttr.AttributeValue);
                    capital.Value += finalPayoff - investment.Value;
                    capitalAttr.AttributeValue = JsonSerializer.Serialize(capital);
                    capitalAttr.UpdatedAt = DateTime.UtcNow;
                    _logger.LogDebug("Updated player capital to {Capital} for caravan {CaravanName}.", capital.Value, caravan.Name);
                }
            }

            statusAttr.AttributeValue = "{\"value\": \"Completed\"}";
            statusAttr.UpdatedAt = DateTime.UtcNow;
            _logger.LogDebug("Caravan {CaravanName} status updated to Completed.", caravan.Name);

            var payoffAttr = new EntityAttribute
            {
                EntityId = caravan.Id,
                AttributeKey = "Payoff",
                AttributeValue = JsonSerializer.Serialize(new { Value = finalPayoff }),
                UpdatedAt = DateTime.UtcNow
            };
            _context.EntityAttributes.Add(payoffAttr);
            _logger.LogDebug("Logged payoff {Payoff} for caravan {CaravanName}.", finalPayoff, caravan.Name);
        }
    }
}