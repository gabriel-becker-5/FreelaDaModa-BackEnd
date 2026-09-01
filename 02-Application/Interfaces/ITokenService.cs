namespace _02_Application.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(string user, List<string> allUserRoles);
    }
}