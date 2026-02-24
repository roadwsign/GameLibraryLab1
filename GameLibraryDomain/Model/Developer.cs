using System;
using System.Collections.Generic;

namespace GameLibraryDomain.Model;

public partial class Developer: Entity
{

    public string Name { get; set; } = null!;

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
