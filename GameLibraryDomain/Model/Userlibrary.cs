using System;
using System.Collections.Generic;

namespace GameLibraryDomain.Model;

public partial class Userlibrary: Entity
{

    public string Userid { get; set; } = null!;

    public int Gameid { get; set; }

    public int Statusid { get; set; }

    public int? Rating { get; set; }

    public string? Review { get; set; }

    public bool Isfavorite { get; set; }

    public DateTime Addedat { get; set; }

    public DateTime? Updatedat { get; set; }
    public virtual Game Game { get; set; } = null!;

    public virtual Gamestatus Status { get; set; } = null!;

    public virtual ICollection<Statushistory> Statushistories { get; set; } = new List<Statushistory>();
}
