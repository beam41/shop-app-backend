using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using ShopAppBackend.Models.DTOs;
using ShopAppBackend.Settings.Interfaces;

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
                password,
                Encoding.ASCII.GetBytes(_userSettings.PasswordSalt),
                KeyDerivationPrf.HMACSHA1,
                10000,
                32));
        }

        public void GenToken(UserLoginDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_userSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
        }
    }
}
