using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using ShopAppBackend.Models;
using ShopAppBackend.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShopAppBackend.Services
{
    public class AuthService
    {
        private readonly IUserSettings _userSettings;

        public AuthService(IUserSettings userSettings)
        {
            _userSettings = userSettings;
        }

        public string HashPassword(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(_userSettings.PasswordSalt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32));
        }

        public void GenToken(UserLoginDTO user)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_userSettings.Secret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
        }
    }
}
