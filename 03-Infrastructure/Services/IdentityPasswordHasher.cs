using _04_Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace _03_Infrastructure.Services
{
    public class IdentityPasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string password)
        {
            return _hasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return _hasher.VerifyHashedPassword(null!, passwordHash, password) == PasswordVerificationResult.Success;
        }
    }
}