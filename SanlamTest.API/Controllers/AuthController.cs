using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SanlamTest.API.Models;
using SanlamTest.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SanlamTest.Controllers
{
    public class JwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(string secretKey, string issuer, string audience)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateToken(string accountId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, accountId),
            new Claim(CustomClaimTypes.AccountID, accountId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1000),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(AppSettings appSettings)
        {
            var secretKey = appSettings.Jwt.SecretKey;
            var issuer = appSettings.Jwt.Issuer;
            var audience = appSettings.Jwt.Audience;

            _jwtService = new JwtService(secretKey, issuer, audience);
        }

        [HttpPost("generate-token")]
        public IActionResult GenerateToken([FromBody] TokenRequest tokenRequest)
        {
            if (string.IsNullOrEmpty(tokenRequest.AccountId))
            {
                return BadRequest("Invalid account ID");
            }

            var token = _jwtService.GenerateToken(tokenRequest.AccountId);
            return Ok(new { Token = token });
        }
        
    }
    public class TokenRequest
    {
        public string AccountId { get; set; }
    }
}
