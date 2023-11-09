using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automarket.Domain.ViewModels.Account
{
    public class ConfirmViewModel
    {
        [Required(ErrorMessage = "Укажите имя")]
        [MaxLength(120, ErrorMessage = "Имя должно иметь длину меньше 20 символов")]
        [MinLength(3, ErrorMessage = "Имя должно иметь длину больше 3 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Вы должны отправить код")]
        [MinLength(6, ErrorMessage = "минимальная длина кода 6 символов")]
        public string VerifyCode { get; set; }
    }
}
