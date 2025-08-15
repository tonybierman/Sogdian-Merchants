using System;
using System.Collections.Generic;

namespace SilkRoad.Core.Data;

public partial class EntityRelationship
{
    public long Id { get; set; }

    public long GameInstanceId { get; set; }

    public long SourceEntityId { get; set; }

    public long TargetEntityId { get; set; }

    public string RelationshipType { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual GameInstance GameInstance { get; set; } = null!;

    public virtual Entity SourceEntity { get; set; } = null!;

    public virtual Entity TargetEntity { get; set; } = null!;
}
