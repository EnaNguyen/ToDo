namespace ToDo.Features.Users.DTO
{
    public class UserView
    {
        public int Id { get; set; } 
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public int Status { get; set; } = 1;
        public bool TwoFA { get; set; } = false;
    }
}
