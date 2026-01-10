

using Authentication_Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Authentication_Infrastructure.Security
{
    public class IdentityService : IIdentityService
    {
        private readonly PasswordHasher<object> _passwordHasher = new PasswordHasher<object>();

        public string GenerateHash(string password)
        {
            return _passwordHasher.HashPassword("USER", password); 
        }

        public bool VerifyPassword(string password, string hash)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}