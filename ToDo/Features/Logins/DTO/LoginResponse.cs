namespace ToDo.Features.Logins.DTO
{
    public class LoginResponse
    {
        public string Message { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public string? Username { get; set; } = string.Empty;
        public string? Role { get; set; } = string.Empty;
        public int Result { get; set; }
        //0 = fail , 1 = success , 2 = need OTP
    }
    public class TokenString
    {
        public string? AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
    }
}
