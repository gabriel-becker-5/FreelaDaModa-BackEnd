using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs
{
    public class UserUpdateDto
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }
}