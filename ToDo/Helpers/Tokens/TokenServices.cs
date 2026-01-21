using EcommercialAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDo.Data.Entities;
using ToDo.Features.Logins.DTO;
namespace ToDo.Helpers.Tokens
{
    public class TokenServices: ITokenServices
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _http;
        public TokenServices(IConfiguration configuration, ApplicationDbContext context, IHttpContextAccessor http) 
        {
            _configuration = configuration;
            _context = context;
            _http = http;
        }
        public string GenerateAccessToken(int userId, string username, string role)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresMinutes = int.Parse(_configuration["Jwt:AccessTokenExpires"] ?? "60");
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            var refreshTokenString = Convert.ToBase64String(randomBytes);
            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:RefreshTokenExpiresDays"] ?? "7")),
            };
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return refreshTokenString;
        }
        public async Task<string?> ValidateAndRefreshTokenAsync(string refreshTokenString)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshTokenString && !t.IsRevoked && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
            if (token == null) return null;
            var user = await _context.Users.FindAsync(token.UserId);
            if (user == null) return null;
            token.IsUsed = true;
            await _context.SaveChangesAsync();
            return GenerateAccessToken(user.Id, user.Username, user.Role);
        }
        public async Task<TokenString> GenerateBothTokensAsync(
            int userId,
            string username,
            string role)
        {
            var accessToken = GenerateAccessToken(userId, username, role);
            var refreshToken = await GenerateRefreshTokenAsync(userId);
            return new TokenString
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        public void SetTokenInCookie(TokenString token)
        {
            var response = _http.HttpContext?.Response;
            if (response == null) return;

            response.Cookies.Append("AccessToken", token.AccessToken ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60),
                IsEssential = true,
                Path = "/",
            });

            response.Cookies.Append("RefreshToken", token.RefreshToken ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                IsEssential = true,
                Path = "/",
            });
        }
    }
}
