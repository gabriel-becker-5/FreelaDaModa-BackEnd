namespace _04_Domain.Entities.UserInfo
{
    public class Role
    {
        public int Id { get; set; }

        public string RoleName { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}