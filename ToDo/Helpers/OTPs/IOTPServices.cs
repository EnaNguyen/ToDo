using ToDo.Features.Logins.DTO;

namespace ToDo.Helpers.OTPs
{
    public interface IOTPServices
    {
        Task<string> GenerateSecretKey(int userId);
        Task<string> GetDecryptedSecretKey(int userId);
        Task<TokenString> VerifyTotpAsync(int userId, string userInputOtp);
    }
}
