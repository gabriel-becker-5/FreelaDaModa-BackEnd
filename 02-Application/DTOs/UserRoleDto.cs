using System.ComponentModel.DataAnnotations;

namespace _02_Application.DTOs
{
    public class UserRoleDto
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role Name é obrigatório.")]
        public string RoleName { get; set; }
    }
}