namespace ToDo.Features.Users.DTO
{
    public class UserChangePasswordDTO
    {
        public int Id { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string NewPasswordConfirm { get; set; } = string.Empty;
    }
}
