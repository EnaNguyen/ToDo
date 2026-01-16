namespace ToDo.Features.Logins.DTO
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class OTPInput
    {
        public int UserId { get; set; }
        public string OTPCode { get; set; }
    }
}
