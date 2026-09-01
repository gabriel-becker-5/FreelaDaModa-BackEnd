using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs
{
    public class UserRegisterDto
    {
        [EmailAddress, Required(ErrorMessage = "O e-mail é obrigatório.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime BirthDate { get; set; }
    }
}