using EcommercialAPI.Data.Entities;
using ToDo.Data.Entities;
namespace ToDo.Data.Entities
{
    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string Password { get; set; } = string.Empty;
        public int Status { get; set; } = 1; 
        public bool TwoFA { get; set; } = false;
        public string SecretKey { get; set; } = string.Empty;
        public ICollection<ToDo>? ToDos { get; set; } = new List<ToDo>();
        public ICollection<RefreshToken> refreshToken { get; set; } = new List<RefreshToken>();
    }
}
