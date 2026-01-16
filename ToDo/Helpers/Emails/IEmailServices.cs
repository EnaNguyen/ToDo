namespace ToDo.Helpers.Emails
{
    public interface IEmailServices
    {
        Task SendEmail(string email, string otp, string request);
    }
}
