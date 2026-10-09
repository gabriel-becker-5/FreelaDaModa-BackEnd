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
            PasswordVerificationResult result = _hasher.VerifyHashedPassword(null!, passwordHash, password);

            if (result == PasswordVerificationResult.SuccessRehashNeeded ||
                result == PasswordVerificationResult.Success)
            {
                return true;
            }
            return false;
        }
    }
}