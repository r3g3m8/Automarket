using System.ComponentModel.DataAnnotations;

namespace Automarket.Domain.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Укажите имя")]
        [MaxLength(120, ErrorMessage = "Имя должно иметь длину меньше 20 символов")]
        [MinLength(3, ErrorMessage = "Имя должно иметь длину больше 3 символов")]
        public string Name { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Укажите пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен иметь длину больше 6 символов")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Вы должны отправить код")]
        [MinLength(6, ErrorMessage = "минимальная длина кода 6 символов")]
        public string? VerifyCode { get; set; }

        [Required(ErrorMessage = "Вы должны отправить код")]
        [MinLength(6, ErrorMessage = "минимальная длина кода 6 символов")]
        public string? SendedCode { get; set; }
    }
}
