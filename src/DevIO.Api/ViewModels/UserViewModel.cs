using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage ="O Campo {0} é obrigatorio!")]
        [EmailAddress(ErrorMessage = "O campo {0} esta em formato invalido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O Campo {0} é obrigatorio!")]
        [StringLength(100, ErrorMessage = "O Campo {0} precisa estar entre {2} e {1} caracteres", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "As senhas não conferem!")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "O Campo {0} é obrigatorio!")]
        [EmailAddress(ErrorMessage = "O campo {0} esta em formato invalido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O Campo {0} é obrigatorio!")]
        [StringLength(100, ErrorMessage = "O Campo {0} precisa estar entre {2} e {1} caracteres", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
