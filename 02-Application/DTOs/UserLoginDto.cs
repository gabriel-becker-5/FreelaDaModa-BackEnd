using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; }
    }
}