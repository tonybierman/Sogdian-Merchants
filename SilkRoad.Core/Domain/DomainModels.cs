using System;
using System.Collections.Generic;

namespace SilkRoad.Core.Domain
{
    public class Route
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string RiskLevel { get; set; }

        public class Goods
        {
            public string Type { get; set; }
            public int Quantity { get; set; }
        }

        public class Investment
        {
            public double Value { get; set; }
        }

        public class Capital
        {
            public double Value { get; set; }
        }

        public class MarketDemand
        {
            public Dictionary<string, GoodDemand> GoodsDemand { get; set; } = new Dictionary<string, GoodDemand>();
        }

        public class GoodDemand
        {
            public double Price { get; set; }
            public int MaxQuantity { get; set; }
        }

        public class MarketPrices
        {
            public Dictionary<string, Dictionary<string, double>> Prices { get; set; } = new();
        }

        public class RandomEvents
        {
            public Dictionary<string, EventDetail> Events { get; set; } = new();
        }

        public class EventDetail
        {
            public double Probability { get; set; }
            public string Impact { get; set; }
        }
    }

    public class GameInstanceDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string GameType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool? IsActive { get; set; }
        public List<EntityDto> Entities { get; set; }
        public List<EntityRelationshipDto> EntityRelationships { get; set; }
        public List<GameStateDto> GameStates { get; set; }
        public UserDto User { get; set; }
    }

    public class EntityDto
    {
        public long Id { get; set; }
        public long GameInstanceId { get; set; }
        public string EntityType { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; }
        public List<EntityAttributeDto> EntityAttributes { get; set; }
        public List<EntityRelationshipDto> EntityRelationshipSourceEntities { get; set; }
        public List<EntityRelationshipDto> EntityRelationshipTargetEntities { get; set; }
    }

    public class EntityAttributeDto
    {
        public long Id { get; set; }
        public long EntityId { get; set; }
        public string AttributeKey { get; set; }
        public object AttributeValue { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class EntityRelationshipDto
    {
        public long Id { get; set; }
        public long GameInstanceId { get; set; }
        public long SourceEntityId { get; set; }
        public long TargetEntityId { get; set; }
        public string RelationshipType { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class GameStateDto
    {
        public long Id { get; set; }
        public long GameInstanceId { get; set; }
        public string StateKey { get; set; }
        public object StateValue { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}