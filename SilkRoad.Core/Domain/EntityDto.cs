namespace SilkRoad.Core.Domain
{
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
}