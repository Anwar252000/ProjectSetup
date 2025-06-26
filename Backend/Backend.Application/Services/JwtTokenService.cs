using Backend.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Application.Services
{
    public class JwtTokenService : IJwtToken
    {
        private readonly ILogger<JwtTokenService> _logger;
        private readonly IConfiguration _configuration;

        public JwtTokenService(ILogger<JwtTokenService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            try
            {
                // Retrieve the key from configuration and ensure it's properly set
                var key = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(key))
                {
                    throw new InvalidOperationException("JWT Key is not configured.");
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                // Ensure the expiration time is set in configuration
                if (!double.TryParse(_configuration["Jwt:ExpireMinutes"], out double expireMinutes))
                {
                    throw new InvalidOperationException("JWT Expiration time is not configured correctly.");
                }

                // Create the JWT token with the claims
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expireMinutes), // Use UtcNow for expiration
                    signingCredentials: credentials);

                // Log the successful JWT generation
                _logger.LogInformation("JWT token generated successfully for user with claims: {Claims}", string.Join(", ", claims.Select(c => c.Type + ":" + c.Value)));

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating JWT token: {ExceptionMessage}", ex, ex.Message);
                throw;
            }
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
