using System.ComponentModel.DataAnnotations;

namespace GameLibraryInfrastructure.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле Нікнейм є обов'язковим")]
        [Display(Name = "Нікнейм")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Поле Email є обов'язковим")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Поле Пароль є обов'язковим")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Поле Підтвердження паролю є обов'язковим")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        [Display(Name = "Підтвердження паролю")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; } = null!;
    }
}