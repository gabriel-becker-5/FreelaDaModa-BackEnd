using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs.User
{
    public class LoginDto
    {
        [Required(ErrorMessage = "O E-mail é obrigatório."), MaxLength(100)]
        [EmailAddress(ErrorMessage = "Informe um E-mail válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; }
    }
}
