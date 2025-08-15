namespace SilkRoad.Core.Domain
{
    public class GameStateDto
    {
        public long Id { get; set; }
        public long GameInstanceId { get; set; }
        public string StateKey { get; set; }
        public object StateValue { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}