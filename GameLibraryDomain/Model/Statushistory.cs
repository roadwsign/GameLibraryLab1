using System;
using System.Collections.Generic;

namespace GameLibraryDomain.Model;

public partial class Statushistory: Entity
{

    public int Userlibraryid { get; set; }

    public int? Oldstatusid { get; set; }

    public int Newstatusid { get; set; }

    public DateTime Changedate { get; set; }

    public virtual Gamestatus Newstatus { get; set; } = null!;

    public virtual Gamestatus? Oldstatus { get; set; }

    public virtual Userlibrary Userlibrary { get; set; } = null!;
}
