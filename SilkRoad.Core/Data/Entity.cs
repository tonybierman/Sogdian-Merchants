using System;
using System.Collections.Generic;

namespace SilkRoad.Core.Data;

public partial class Entity
{
    public long Id { get; set; }

    public long GameInstanceId { get; set; }

    public string EntityType { get; set; } = null!;

    public string? Name { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual ICollection<EntityAttribute> EntityAttributes { get; set; } = new List<EntityAttribute>();

    public virtual ICollection<EntityRelationship> EntityRelationshipSourceEntities { get; set; } = new List<EntityRelationship>();

    public virtual ICollection<EntityRelationship> EntityRelationshipTargetEntities { get; set; } = new List<EntityRelationship>();

    public virtual GameInstance GameInstance { get; set; } = null!;
}
