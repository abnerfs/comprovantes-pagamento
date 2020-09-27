using ComprovantesPagamento.Domain.Models;
using ComprovantesPagamento.Domain.Responses;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ComprovantesPagamento.Services
{
    public class JwtService
    {
        private JwtConfig _config;

        public JwtService(JwtConfig config)
        {
            _config = config;
        }

        public static readonly string USERID_CLAIM = "user_id";

        public TokenValidationParameters GetTokenValidationParams()
        {
            try
            {
                return new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public JwtResponse GenerateToken(string UserID)
        {
            try
            {
                var claims = new[]
             {
                    new Claim(JwtService.USERID_CLAIM, UserID),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var expiration = DateTime.UtcNow.AddMinutes(_config.ExpiresInMinutes);

                var token = new JwtSecurityToken(
                   issuer: null,
                   audience: null,
                   claims: claims,
                   expires: expiration,
                   signingCredentials: creds);

                var Token = new JwtSecurityTokenHandler().WriteToken(token);

                return new JwtResponse
                {
                    AccessToken = Token,
                    ExpireDate = expiration
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
