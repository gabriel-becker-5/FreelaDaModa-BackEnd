using System.ComponentModel.DataAnnotations;

namespace _04_Domain.Entities.UserInfo
{
    public class User
    {
        public int Id { get; set; }

        [EmailAddress, Required(ErrorMessage = "O E-mail é obrigatório.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório."), MinLength(3), MaxLength(200)]
        public string Name { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime BirthDate { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}