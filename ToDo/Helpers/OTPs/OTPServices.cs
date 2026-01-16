using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore; 
using OtpNet;
using ToDo.Data.Entities;
using ToDo.Features.Logins.DTO;
using ToDo.Helpers.Tokens;

namespace ToDo.Helpers.OTPs
{
    public class OTPServices : IOTPServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;
        private readonly ITokenServices _tokenServices;

        public OTPServices(ApplicationDbContext context, IDataProtectionProvider provider, ITokenServices tokenServices)
        {
            _context = context;
            _protector = provider.CreateProtector("ToDoApp.TotpSecret.v1");
            _tokenServices = tokenServices;
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
        public async Task<TokenString> VerifyTotpAsync(int userId, string userInputOtp)
        {
            string plainSecret;
            try
            {
                plainSecret = await GetDecryptedSecretKey(userId);
            }
            catch
            {
                throw;
            }
            var secretBytes = Base32Encoding.ToBytes(plainSecret);
            var totp = new Totp(secretBytes, step: 300, mode: OtpHashMode.Sha1, totpSize: 6);
            var user = _context.Users.FirstOrDefault(g => g.Id == userId);
            await _tokenServices.GenerateBothTokensAsync(user.Id, user.Username, user.Role);
            bool verified =  totp.VerifyTotp(
                userInputOtp,
                out _,
                new VerificationWindow(previous: 1, future: 1) 
            );
            if (verified)
            {
               return await _tokenServices.GenerateBothTokensAsync(user.Id, user.Username, user.Role);
            }
            else
            {
                throw new Exception("Invalid OTP");
            }    
        }
    }
}