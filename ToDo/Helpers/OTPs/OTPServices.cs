using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore; 
using OtpNet;
using ToDo.Data.Entities;
using ToDo.Features.Logins.DTO;
using ToDo.Features.Logins.Services;
using ToDo.Helpers.Tokens;
using ToDo.Helpers.Emails;
namespace ToDo.Helpers.OTPs
{
    public class OTPServices : IOTPServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;
        private readonly ITokenServices _tokenServices;
        private readonly IEmailServices _emailServices;
        public OTPServices(ApplicationDbContext context, IDataProtectionProvider provider, ITokenServices tokenServices, IEmailServices services)
        {
            _context = context;
            _protector = provider.CreateProtector("ToDoApp.TotpSecret.v1");
            _tokenServices = tokenServices;
            _emailServices = services;
        }

        public async Task<string> GenerateSecretKey(int userId)
        {
            var secretBytes = KeyGeneration.GenerateRandomKey(20); 
            var base32Secret = Base32Encoding.ToString(secretBytes);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            try
            {
                user.SecretKey = _protector.Protect(base32Secret);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return base32Secret;
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> GetDecryptedSecretKey(int userId)
        {
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || string.IsNullOrEmpty(user.SecretKey))
            {
                throw new Exception("User or SecretKey not found");
            }
            return _protector.Unprotect(user.SecretKey);
        }
        public async Task<LoginResponse> VerifyTotpAsync(OTPRequest request)
        {
            if(!string.IsNullOrEmpty(request.OTPCode))
            {
                string plainSecret;
                try
                {
                    plainSecret = await GetDecryptedSecretKey(request.UserId);
                }
                catch
                {
                    throw;
                }
                var secretBytes = Base32Encoding.ToBytes(plainSecret);
                var totp = new Totp(secretBytes, step: 300, mode: OtpHashMode.Sha1, totpSize: 6);
                var user = _context.Users.FirstOrDefault(g => g.Id == request.UserId);
                await _tokenServices.GenerateBothTokensAsync(user.Id, user.Username, user.Role);
                bool verified = totp.VerifyTotp(
                    request.OTPCode,
                    out _,
                    new VerificationWindow(previous: 1, future: 1)
                );
                if (verified)
                {
                    var TokenGen = await _tokenServices.GenerateBothTokensAsync(user.Id, user.Username, user.Role);
                    _tokenServices.SetTokenInCookie(TokenGen);
                    return new LoginResponse
                    {
                        Message = "OTP Verified Successfully",
                        UserId = user.Id,
                        FullName = user.FullName,
                        Username = user.Username,
                        Role = user.Role,
                        Result = 1,
                        
                    };
                }
                else
                {
                    throw new Exception("Invalid OTP");
                }
            }
            throw new Exception("Invalid OTP");
        }
        public async Task<bool> ReSentOTP(int userId)
        {
            try
            {
                var userAttempLogin = _context.Users.FirstOrDefault(p => p.Id == userId);
                string plainSecret = await GetDecryptedSecretKey(userId);
                var secretBytes = Base32Encoding.ToBytes(plainSecret);
                var totp = new Totp(secretBytes, step: 300, mode: OtpHashMode.Sha1, totpSize: 6);
                string currentOtp = totp.ComputeTotp();
                await _emailServices.SendEmail(userAttempLogin.Email, currentOtp, "Xác thực 2 yếu tố qua Email");
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}