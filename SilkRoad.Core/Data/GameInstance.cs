using System;
using System.Collections.Generic;

namespace SilkRoad.Core.Data;

public partial class GameInstance
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string GameType { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastUpdated { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Entity> Entities { get; set; } = new List<Entity>();

    public virtual ICollection<EntityRelationship> EntityRelationships { get; set; } = new List<EntityRelationship>();

    public virtual ICollection<GameState> GameStates { get; set; } = new List<GameState>();

    public virtual User User { get; set; } = null!;
}
