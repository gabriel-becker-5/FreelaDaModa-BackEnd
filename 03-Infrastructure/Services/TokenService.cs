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

        public TokenService(IConfiguration config) => _JwtKey = config["Jwt:Secret"];

        public string GenerateToken(string user, List<string> allUserRoles)
        {
            List<Claim> claims = [];

            Claim? claimName = new Claim(JwtRegisteredClaimNames.Name, user);
            claims.Add(claimName);

            for (int i = 0; i < allUserRoles.Count; i++)
            {
                Claim? claimRole = new Claim(ClaimTypes.Role, allUserRoles[i]);
                claims.Add(claimRole);
            }

            SymmetricSecurityKey? key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtKey));
            SigningCredentials? creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken? token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}