using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameLibraryDomain.Model;

public partial class Developer: Entity
{
    [Display(Name = "Розробник")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
