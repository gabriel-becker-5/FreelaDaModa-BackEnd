namespace _02_Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string user, ICollection<string> allUserRoles);
    }
}