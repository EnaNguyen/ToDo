
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using System.Security.Cryptography;
using System.Text;
using ToDo.Data.Entities;
using ToDo.Helpers.Emails;
using ToDo.Helpers.OTPs;
using ToDo.Helpers.Tokens;
using ToDo.Features.Logins.DTO;
using static System.Net.WebRequestMethods;

namespace ToDo.Features.Logins.Services
{

    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailServices _emailServices;
        private readonly ITokenServices _tokenServices;
        private readonly IOTPServices _otpServices;
       public AuthenticationServices(ApplicationDbContext context, IEmailServices emailServices, ITokenServices tokenServices, IOTPServices otpServices)
        {
            _context = context;
            _emailServices = emailServices;
            _tokenServices = tokenServices;
            _otpServices = otpServices;
        }
        public async Task<LoginResponse> Login(string username, string password)
        {
            try
            {
                var userAttempLogin = _context.Users.FirstOrDefault(p => p.Username == username || p.Email == username);
                if (userAttempLogin != null)
                {
                    if(userAttempLogin.Password!=HashCode(password))
                        return new LoginResponse
                        {
                            Message = "Login Failed",
                            Result = 0
                        };
                    if (userAttempLogin.TwoFA == true)
                    {
                        if (string.IsNullOrWhiteSpace(userAttempLogin.SecretKey))
                        {
                            await _otpServices.GenerateSecretKey(userAttempLogin.Id);
                        }    
                        string plainSecret = await _otpServices.GetDecryptedSecretKey(userAttempLogin.Id);
                        var secretBytes = Base32Encoding.ToBytes(plainSecret);
                        var totp = new Totp(secretBytes, step: 86400, mode: OtpHashMode.Sha1, totpSize: 6);
                        string currentOtp = totp.ComputeTotp();
                        await _emailServices.SendEmail(userAttempLogin.Email, currentOtp, "Xác thực 2 yếu tố qua Email");
                        return new LoginResponse
                        {
                            Message = "OTP has been sent, please check our email",
                            UserId = userAttempLogin.Id,
                            Result = 2
                        };
                    }
                    var token = await _tokenServices.GenerateBothTokensAsync(userAttempLogin.Id, userAttempLogin.Username, userAttempLogin.Role);
                    _tokenServices.SetTokenInCookie(token);
                    return new LoginResponse
                    {
                        Message = "Login Successfully",
                        UserId = userAttempLogin.Id,
                        FullName = userAttempLogin.FullName,
                        Username = userAttempLogin.Username,
                        Role = userAttempLogin.Role,
                        Result = 1
                    };
                }
                ;
                return new LoginResponse
                {
                    Message = "Login Failed",
                    Result = 0
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Message = "Login Failed",
                    Result = 0
                };
            }
        }

        public string HashCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return string.Empty;
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(code);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
        public string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }     
    }
}
