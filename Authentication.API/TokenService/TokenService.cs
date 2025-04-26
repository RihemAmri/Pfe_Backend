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
            var adminId = "6807f3958d2732dd1864cd9b";
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
    
    public string GenerateResetToken(Utilisateur utilisateur)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.Email, utilisateur.Email),
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("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"));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "YourIssuer",
        audience: "YourAudience",
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
}
}


