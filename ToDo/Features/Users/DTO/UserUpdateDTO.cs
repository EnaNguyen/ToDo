namespace ToDo.Features.Users.DTO
{
    public class UserUpdateDTO
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Status { get; set; } = 1;
        public bool TwoFA { get; set; } = false;
        public string? Role { get; set; } = "User";
    }
}
