using System;
using System.Collections.Generic;

namespace GameLibraryDomain.Model;

public partial class Gamestatus: Entity
{

    public string Statusname { get; set; } = null!;

    public virtual ICollection<Statushistory> StatushistoryNewstatuses { get; set; } = new List<Statushistory>();

    public virtual ICollection<Statushistory> StatushistoryOldstatuses { get; set; } = new List<Statushistory>();

    public virtual ICollection<Userlibrary> Userlibraries { get; set; } = new List<Userlibrary>();
}
