namespace SilkRoad.Core.Domain
{
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
}