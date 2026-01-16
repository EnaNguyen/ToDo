using System.Security.Cryptography;
using System.Threading.Tasks;
using OtpNet;
using ToDo.Data.Entities;
namespace ToDo.Helpers.OTPs
{
    public class OTPServices : IOTPServices
    {
        private readonly ApplicationDbContext _context;
        public OTPServices(ApplicationDbContext context)
        {
               _context = context;
        }
        public async Task<string> GenerateSecretKey(int UserId)
        {
            var secretBytes = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(secretBytes);
            var user = _context.Users.FirstOrDefault(u => u.Id == UserId);
            if (user != null)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    user.SecretKey = base32Secret;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return user.SecretKey;
                }
                catch                 {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            throw new Exception("User not found");
        }
    }
}
