namespace SilkRoad.Core.Domain
{
    public class EntityRelationshipDto
    {
        public long Id { get; set; }
        public long GameInstanceId { get; set; }
        public long SourceEntityId { get; set; }
        public long TargetEntityId { get; set; }
        public string RelationshipType { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}