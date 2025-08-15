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
}