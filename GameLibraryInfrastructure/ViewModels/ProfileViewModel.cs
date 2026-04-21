using GameLibraryDomain.Model;
using System.ComponentModel.DataAnnotations;

namespace GameLibraryInfrastructure.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Поле Нікнейм є обов'язковим")]
        [Display(Name = "Новий нікнейм")]
        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Поточний пароль")]
        public string? OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Новий пароль")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження нового пароля")]
        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають")]
        public string? ConfirmPassword { get; set; }

        public List<Statushistory> History { get; set; } = new();
    }
}