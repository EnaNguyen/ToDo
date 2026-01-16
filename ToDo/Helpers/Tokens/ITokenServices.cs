using ToDo.Features.Logins.DTO;
namespace ToDo.Helpers.Tokens
{
    public interface ITokenServices
    {
        string GenerateAccessToken(int userId, string username, string role);
        Task<string> GenerateRefreshTokenAsync(int userId);
        Task<string?> ValidateAndRefreshTokenAsync(string refreshToken);
        Task<TokenString> GenerateBothTokensAsync(int userId, string username, string role);
    }
}
