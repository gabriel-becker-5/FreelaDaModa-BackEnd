namespace _02_Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(int userId, string user, ICollection<string> allUserRoles);
    }
}