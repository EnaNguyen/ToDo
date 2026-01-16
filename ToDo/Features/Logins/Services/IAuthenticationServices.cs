using ToDo.Features.Logins.DTO;

namespace ToDo.Features.Logins.Services
{
    public interface IAuthenticationServices
    {
        Task<LoginResponse> Login (string username, string password);
        string HashCode(string code);
        string GenerateOtp();
    }
}
