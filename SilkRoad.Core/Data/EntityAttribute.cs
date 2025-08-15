using System;
using System.Collections.Generic;

namespace SilkRoad.Core.Data;

public partial class EntityAttribute
{
    public long Id { get; set; }

    public long EntityId { get; set; }

    public string AttributeKey { get; set; } = null!;

    public string AttributeValue { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public virtual Entity Entity { get; set; } = null!;
}
