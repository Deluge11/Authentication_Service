using Authentication_Core.Interfaces;
using Authentication_Infrastructure.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Infrastructure.Security
{
    public class TokenService : ITokenService
    {
        public TokenService(JwtOptions jwtOptions)
        {
            JwtOptions = jwtOptions;
        }

        public JwtOptions JwtOptions { get; }

        public string GenerateJwtToken(int userId, long permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("permissions", permissions.ToString())

            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = JwtOptions.Issuer,
                Audience = JwtOptions.Audience,
                Expires = DateTime.UtcNow.AddMinutes(JwtOptions.LifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SigningKey)), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }


    }
}
