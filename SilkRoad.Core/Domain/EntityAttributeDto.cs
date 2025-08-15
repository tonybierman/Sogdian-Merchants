namespace SilkRoad.Core.Domain
{
    public class EntityAttributeDto
    {
        public long Id { get; set; }
        public long EntityId { get; set; }
        public string AttributeKey { get; set; }
        public object AttributeValue { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}