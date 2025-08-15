using System;
using System.Collections.Generic;

namespace SilkRoad.Core.Data;

public partial class GameState
{
    public long Id { get; set; }

    public long GameInstanceId { get; set; }

    public string StateKey { get; set; } = null!;

    public string StateValue { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public virtual GameInstance GameInstance { get; set; } = null!;
}
