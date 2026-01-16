namespace ToDo.Features.Users.DTO
{
    public class UserCreateDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordConfirm { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public bool TwoFA { get; set; } = false;
    }
}
