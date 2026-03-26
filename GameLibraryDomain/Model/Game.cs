using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameLibraryDomain.Model;

public partial class Game: Entity
{
    [Display(Name ="Назва гри")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]

    public string Title { get; set; } = null!;

    [Display(Name = "Посилання на постер гри")]
    public string? Posterurl { get; set; }

    [Display(Name = "Опис гри")]

    public string? Description { get; set; }

    [Display(Name = "Рік випуску")]
    public int? Releaseyear { get; set; }

    [Display(Name = "Жанр")]

    public int Genreid { get; set; }

    [Display(Name = "Розробник")]

    public int Developerid { get; set; }

    [Display(Name = "Додано")]
    public DateTime Createdat { get; set; }

    [Display(Name = "Оновлено")]

    public DateTime Updatedat { get; set; }

    [Display(Name = "Розробник")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]

    public virtual Developer Developer { get; set; } = null!;

    [Display(Name = "Жанр")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public virtual Genre Genre { get; set; } = null!;

    public virtual ICollection<Userlibrary> Userlibraries { get; set; } = new List<Userlibrary>();
}
