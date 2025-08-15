using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SilkRoad.Core.Data;
using SilkRoad.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilkRoad.Core.Data
{
    public class SilkRoadDataSeeder : DataSeeder, ISeedData
    {
        private readonly ILogger<SilkRoadDataSeeder> _logger;

        public SilkRoadDataSeeder(GameDbContext context, IGameService gameService, ILogger<SilkRoadDataSeeder> logger)
            : base(context, gameService)
        {
            _logger = logger;
        }

        public override async Task SeedDataAsync()
        {
            await SeedSilkRoadDataAsync();
        }

        public async Task SeedSilkRoadDataAsync()
        {
            _logger.LogInformation("Starting SilkRoad data seeding...");

            // Seed User
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == "player1");
            if (user == null)
            {
                user = new User
                {
                    Username = "player1",
                    Email = "player1@example.com",
                    PasswordHash = "hashed_password"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created user: {Username}", user.Username);
            }

            // Seed GameInstance
            var gameInstance = await _context.GameInstances
                .FirstOrDefaultAsync(gi => gi.UserId == user.Id && gi.GameType == "silkroad");
            if (gameInstance == null)
            {
                gameInstance = new GameInstance
                {
                    UserId = user.Id,
                    GameType = "silkroad",
                    IsActive = true
                };
                _context.GameInstances.Add(gameInstance);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created GameInstance: {GameType}, ID: {GameInstanceId}", gameInstance.GameType, gameInstance.Id);
            }

            // Seed Entities
            var entities = await _context.Entities
                .Where(e => e.GameInstanceId == gameInstance.Id)
                .ToListAsync();

            // Player Entity
            var player = entities.FirstOrDefault(e => e.EntityType == "Merchant" && e.Name == "Player");
            if (player == null)
            {
                player = new Entity
                {
                    GameInstanceId = gameInstance.Id,
                    Name = "Player",
                    EntityType = "Merchant",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Entities.Add(player);
                _logger.LogInformation("Created Player entity: {EntityName}", player.Name);
            }

            // Rival Merchant
            var rival = entities.FirstOrDefault(e => e.EntityType == "Merchant" && e.Name == "Rival1");
            if (rival == null)
            {
                rival = new Entity
                {
                    GameInstanceId = gameInstance.Id,
                    Name = "Rival1",
                    EntityType = "Merchant",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Entities.Add(rival);
                _logger.LogInformation("Created Rival entity: {EntityName}", rival.Name);
            }

            // Caravans
            var caravan1 = entities.FirstOrDefault(e => e.EntityType == "Caravan" && e.Name == "SG-001");
            if (caravan1 == null)
            {
                caravan1 = new Entity
                {
                    GameInstanceId = gameInstance.Id,
                    Name = "SG-001",
                    EntityType = "Caravan",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Entities.Add(caravan1);
                _logger.LogInformation("Created Caravan entity: {EntityName}", caravan1.Name);
            }

            var caravan2 = entities.FirstOrDefault(e => e.EntityType == "Caravan" && e.Name == "SG-002");
            if (caravan2 == null)
            {
                caravan2 = new Entity
                {
                    GameInstanceId = gameInstance.Id,
                    Name = "SG-002",
                    EntityType = "Caravan",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Entities.Add(caravan2);
                _logger.LogInformation("Created Caravan entity: {EntityName}", caravan2.Name);
            }

            // Nomadic Tribe
            var tribe = entities.FirstOrDefault(e => e.EntityType == "Tribe" && e.Name == "TurkicTribe");
            if (tribe == null)
            {
                tribe = new Entity
                {
                    GameInstanceId = gameInstance.Id,
                    Name = "TurkicTribe",
                    EntityType = "Tribe",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Entities.Add(tribe);
                _logger.LogInformation("Created Tribe entity: {EntityName}", tribe.Name);
            }

            // Local Ruler
            var ruler = entities.FirstOrDefault(e => e.EntityType == "Ruler" && e.Name == "ChangAnRuler");
            if (ruler == null)
            {
                ruler = new Entity
                {
                    GameInstanceId = gameInstance.Id,
                    Name = "ChangAnRuler",
                    EntityType = "Ruler",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Entities.Add(ruler);
                _logger.LogInformation("Created Ruler entity: {EntityName}", ruler.Name);
            }

            // Markets
            var changAnMarket = entities.FirstOrDefault(e => e.EntityType == "Market" && e.Name == "ChangAnMarket");
            if (changAnMarket == null)
            {
                changAnMarket = new Entity
                {
                    GameInstanceId = gameInstance.Id,
                    Name = "ChangAnMarket",
                    EntityType = "Market",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Entities.Add(changAnMarket);
                _logger.LogInformation("Created Market entity: {EntityName}", changAnMarket.Name);
            }

            var damascusMarket = entities.FirstOrDefault(e => e.EntityType == "Market" && e.Name == "DamascusMarket");
            if (damascusMarket == null)
            {
                damascusMarket = new Entity
                {
                    GameInstanceId = gameInstance.Id,
                    Name = "DamascusMarket",
                    EntityType = "Market",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Entities.Add(damascusMarket);
                _logger.LogInformation("Created Market entity: {EntityName}", damascusMarket.Name);
            }

            await _context.SaveChangesAsync();

            // Seed EntityAttributes
            var attributes = new List<EntityAttribute>
            {
                // Player Attributes
                new EntityAttribute
                {
                    EntityId = player.Id,
                    AttributeKey = "Capital",
                    AttributeValue = "{\"value\": 1000}",
                    UpdatedAt = DateTime.UtcNow
                },
                new EntityAttribute
                {
                    EntityId = player.Id,
                    AttributeKey = "Reputation",
                    AttributeValue = "{\"value\": 0.8}",
                    UpdatedAt = DateTime.UtcNow
                },
                // Rival Attributes
                new EntityAttribute
                {
                    EntityId = rival.Id,
                    AttributeKey = "Capital",
                    AttributeValue = "{\"value\": 800}",
                    UpdatedAt = DateTime.UtcNow
                },
                new EntityAttribute
                {
                    EntityId = rival.Id,
                    AttributeKey = "Reputation",
                    AttributeValue = "{\"value\": 0.6}",
                    UpdatedAt = DateTime.UtcNow
                },
                // Caravan SG-001 Attributes
                new EntityAttribute
                {
                    EntityId = caravan1.Id,
                    AttributeKey = "Goods",
                    AttributeValue = "{\"type\": \"Silk\", \"quantity\": 50}",
                    UpdatedAt = DateTime.UtcNow
                },
                new EntityAttribute
                {
                    EntityId = caravan1.Id,
                    AttributeKey = "Route",
                    AttributeValue = "{\"start\": \"Samarkand\", \"end\": \"ChangAn\", \"riskLevel\": \"High\"}",
                    UpdatedAt = DateTime.UtcNow
                },
                new EntityAttribute
                {
                    EntityId = caravan1.Id,
                    AttributeKey = "Investment",
                    AttributeValue = "{\"value\": 200}",
                    UpdatedAt = DateTime.UtcNow
                },
                new EntityAttribute
                {
                    EntityId = caravan1.Id,
                    AttributeKey = "Status",
                    AttributeValue = "{\"value\": \"InTransit\"}",
                    UpdatedAt = DateTime.UtcNow
                },
                // Caravan SG-002 Attributes
                new EntityAttribute
                {
                    EntityId = caravan2.Id,
                    AttributeKey = "Goods",
                    AttributeValue = "{\"type\": \"Spices\", \"quantity\": 20}",
                    UpdatedAt = DateTime.UtcNow
                },
                new EntityAttribute
                {
                    EntityId = caravan2.Id,
                    AttributeKey = "Route",
                    AttributeValue = "{\"start\": \"Bukhara\", \"end\": \"Damascus\", \"riskLevel\": \"Medium\"}",
                    UpdatedAt = DateTime.UtcNow
                },
                new EntityAttribute
                {
                    EntityId = caravan2.Id,
                    AttributeKey = "Investment",
                    AttributeValue = "{\"value\": 150}",
                    UpdatedAt = DateTime.UtcNow
                },
                new EntityAttribute
                {
                    EntityId = caravan2.Id,
                    AttributeKey = "Status",
                    AttributeValue = "{\"value\": \"InTransit\"}",
                    UpdatedAt = DateTime.UtcNow
                },
                // Tribe Attributes
                new EntityAttribute
                {
                    EntityId = tribe.Id,
                    AttributeKey = "TollRate",
                    AttributeValue = "{\"value\": 50}",
                    UpdatedAt = DateTime.UtcNow
                },
                new EntityAttribute
                {
                    EntityId = tribe.Id,
                    AttributeKey = "Aggression",
                    AttributeValue = "{\"value\": 0.3}",
                    UpdatedAt = DateTime.UtcNow
                },
                // Ruler Attributes
                new EntityAttribute
                {
                    EntityId = ruler.Id,
                    AttributeKey = "TaxRate",
                    AttributeValue = "{\"value\": 0.1}",
                    UpdatedAt = DateTime.UtcNow
                },
                // Market Attributes
                new EntityAttribute
                {
                    EntityId = changAnMarket.Id,
                    AttributeKey = "Demand",
                    AttributeValue = "{\"Silk\": {\"price\": 12, \"maxQuantity\": 100}, \"Spices\": {\"price\": 8, \"maxQuantity\": 50}}",
                    UpdatedAt = DateTime.UtcNow
                },
                new EntityAttribute
                {
                    EntityId = damascusMarket.Id,
                    AttributeKey = "Demand",
                    AttributeValue = "{\"Silk\": {\"price\": 30, \"maxQuantity\": 80}, \"Spices\": {\"price\": 15, \"maxQuantity\": 40}}",
                    UpdatedAt = DateTime.UtcNow
                }
            };

            foreach (var attr in attributes)
            {
                var existing = await _context.EntityAttributes
                    .FirstOrDefaultAsync(ea => ea.EntityId == attr.EntityId && ea.AttributeKey == attr.AttributeKey);
                if (existing == null)
                {
                    _context.EntityAttributes.Add(attr);
                    _logger.LogDebug("Added attribute {AttributeKey} for entity ID {EntityId}", attr.AttributeKey, attr.EntityId);
                }
            }
            await _context.SaveChangesAsync();

            // Seed EntityRelationships
            var relationships = new List<EntityRelationship>
            {
                // Player partners with Rival for SG-001 (Prisoner's Dilemma)
                new EntityRelationship
                {
                    GameInstanceId = gameInstance.Id,
                    SourceEntityId = player.Id,
                    TargetEntityId = rival.Id,
                    RelationshipType = "Partnership",
                    CreatedAt = DateTime.UtcNow
                },
                // Caravan SG-001 negotiates with Tribe
                new EntityRelationship
                {
                    GameInstanceId = gameInstance.Id,
                    SourceEntityId = caravan1.Id,
                    TargetEntityId = tribe.Id,
                    RelationshipType = "TollNegotiation",
                    CreatedAt = DateTime.UtcNow
                },
                // Caravan SG-001 targets ChangAnMarket
                new EntityRelationship
                {
                    GameInstanceId = gameInstance.Id,
                    SourceEntityId = caravan1.Id,
                    TargetEntityId = changAnMarket.Id,
                    RelationshipType = "Trade",
                    CreatedAt = DateTime.UtcNow
                },
                // Caravan SG-002 negotiates with Tribe
                new EntityRelationship
                {
                    GameInstanceId = gameInstance.Id,
                    SourceEntityId = caravan2.Id,
                    TargetEntityId = tribe.Id,
                    RelationshipType = "TollNegotiation",
                    CreatedAt = DateTime.UtcNow
                },
                // Caravan SG-002 targets DamascusMarket
                new EntityRelationship
                {
                    GameInstanceId = gameInstance.Id,
                    SourceEntityId = caravan2.Id,
                    TargetEntityId = damascusMarket.Id,
                    RelationshipType = "Trade",
                    CreatedAt = DateTime.UtcNow
                }
            };

            foreach (var rel in relationships)
            {
                var existing = await _context.EntityRelationships
                    .FirstOrDefaultAsync(er => er.GameInstanceId == rel.GameInstanceId &&
                                               er.SourceEntityId == rel.SourceEntityId &&
                                               er.TargetEntityId == rel.TargetEntityId &&
                                               er.RelationshipType == rel.RelationshipType);
                if (existing == null)
                {
                    _context.EntityRelationships.Add(rel);
                    _logger.LogDebug("Added relationship {RelationshipType} for SourceEntityId {SourceId} to TargetEntityId {TargetId}",
                        rel.RelationshipType, rel.SourceEntityId, rel.TargetEntityId);
                }
            }
            await _context.SaveChangesAsync();

            // Seed GameState
            var gameStates = new List<GameState>
            {
                new GameState
                {
                    GameInstanceId = gameInstance.Id,
                    StateKey = "MarketPrices",
                    StateValue = "{\"ChangAn\": {\"Silk\": 12, \"Spices\": 8}, \"Damascus\": {\"Silk\": 30, \"Spices\": 15}}",
                    UpdatedAt = DateTime.UtcNow
                },
                new GameState
                {
                    GameInstanceId = gameInstance.Id,
                    StateKey = "RandomEvents",
                    StateValue = "{\"BanditAttack\": {\"probability\": 0.3, \"impact\": \"Lose 50% goods\"}, \"Sandstorm\": {\"probability\": 0.2, \"impact\": \"Lose 30% goods\"}}",
                    UpdatedAt = DateTime.UtcNow
                }
            };

            foreach (var state in gameStates)
            {
                var existing = await _context.GameStates
                    .FirstOrDefaultAsync(gs => gs.GameInstanceId == state.GameInstanceId && gs.StateKey == state.StateKey);
                if (existing == null)
                {
                    _context.GameStates.Add(state);
                    _logger.LogDebug("Added GameState {StateKey} for GameInstance {GameInstanceId}", state.StateKey, state.GameInstanceId);
                }
            }
            await _context.SaveChangesAsync();

            _logger.LogInformation("SilkRoad data seeding completed successfully!");
        }
    }
}