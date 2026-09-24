using _02_Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _03_Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _JwtKey;

        public TokenService(IConfiguration config) =>
            _JwtKey = config["Jwt:Key"] // <-- Alterado de "Jwt:Secret" para "Jwt:Key"
                      ?? throw new InvalidOperationException("Jwt:Key não configurado.");

        public string GenerateToken(int userId, string user, ICollection<string> allUserRoles)
        {
            List<Claim> claims = [];

            Claim claimName = new Claim(JwtRegisteredClaimNames.Name, user);
            claims.Add(claimName);

            Claim claimId = new Claim(JwtRegisteredClaimNames.NameId, userId.ToString());
            claims.Add(claimId);

            foreach (var userRole in allUserRoles)
            {
                Claim claimRole = new Claim("roles", userRole);
                claims.Add(claimRole);
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtKey));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}