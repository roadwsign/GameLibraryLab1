using System;
using System.Collections.Generic;

namespace GameLibraryDomain.Model;

public partial class Game: Entity
{

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? Releaseyear { get; set; }

    public int Genreid { get; set; }

    public int Developerid { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public virtual Developer Developer { get; set; } = null!;

    public virtual Genre Genre { get; set; } = null!;

    public virtual ICollection<Userlibrary> Userlibraries { get; set; } = new List<Userlibrary>();
}
