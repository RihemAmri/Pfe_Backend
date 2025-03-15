using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Authentication.API.Entities;
using System.Security.Claims;

namespace Authentication.API.Services
{
    public class TokenService
    {
        private readonly string _secretKey;

        public TokenService(string secretKey)
        {
            _secretKey = secretKey;
        }

        public string GenerateToken(Utilisateur utilisateur)
        {
            // Crée les informations de l'utilisateur pour le token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, utilisateur.Id.ToString()),
                new Claim(ClaimTypes.Name, utilisateur.Nom),
                new Claim(ClaimTypes.Email, utilisateur.Email),
                new Claim(ClaimTypes.Role, utilisateur.Role)
            };

           
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crée le token avec une expiration de 1 jour
            var token = new JwtSecurityToken(
                issuer: "MyApp", 
                audience: "MyAppUsers", 
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            // Retourne le token sous forme de chaîne
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
