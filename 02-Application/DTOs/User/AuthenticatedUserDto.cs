namespace _02_Application.DTOs.User
{
    public class AuthenticatedUserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}
