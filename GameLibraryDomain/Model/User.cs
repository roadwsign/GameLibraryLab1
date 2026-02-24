using System;
using System.Collections.Generic;

namespace GameLibraryDomain.Model;

public partial class User: Entity
{

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime Createdat { get; set; }

    public virtual ICollection<Userlibrary> Userlibraries { get; set; } = new List<Userlibrary>();
}
