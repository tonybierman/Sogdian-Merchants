using System;
using System.Collections.Generic;

namespace SilkRoad.Core.Data;

public partial class User
{
    public long Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public virtual ICollection<GameInstance> GameInstances { get; set; } = new List<GameInstance>();
}
