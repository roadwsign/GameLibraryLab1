using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameLibraryDomain.Model;

public partial class Genre: Entity
{
    [Display(Name = "Назва жанру")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
