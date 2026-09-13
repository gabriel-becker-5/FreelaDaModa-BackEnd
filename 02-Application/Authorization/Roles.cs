namespace _02_Application.Authorization
{
    public static class Roles
    {
        public static readonly IReadOnlyList<string> All = [Admin, Company, Freelancer];
        public const string Admin = "Admin";
        public const string Company = "Empresa";
        public const string Freelancer = "Freelancer";
    }
}