using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using A2TP3.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace A2TP3.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Usuario usuario)
        {
            // Obter a chave secreta do appsettings.json
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            // Credenciais de assinatura do token
            var cred = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            // Claims (informações) que vão no token, ex: Id do usuário e nome
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Login),
                // Outros claims se necessário
            };

            // Criar o token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],    // Opcional
                audience: _configuration["Jwt:Audience"],// Opcional
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),    // Tempo de expiração do token
                signingCredentials: cred
            );

            // Serializar o token para string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
