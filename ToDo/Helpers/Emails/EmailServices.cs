
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
namespace ToDo.Helpers.Emails
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public EmailServices(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        public async Task SendEmail(string email, string otp, string request)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                if (string.IsNullOrEmpty(emailSettings["SmtpServer"]) ||
                    string.IsNullOrEmpty(emailSettings["SenderEmail"]) ||
                    string.IsNullOrEmpty(emailSettings["SenderPassword"]))
                {
                    return;
                }
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                emailSettings["SenderName"] ?? "EnaNguyen",
                emailSettings["SenderEmail"]));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = request;
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = "";
                string templatePath = "";
                string cssPath = "";
                try
                {
                    templatePath = Path.Combine(_env.WebRootPath, "template", "validation", "validation.html");
                    cssPath = Path.Combine(_env.WebRootPath, "template", "validation", "validation.css");
                    bodyBuilder.TextBody = $"Mã OTP của bạn là: {otp}. Có hiệu lực trong 5 phút.";
                }
                catch(Exception ex)
                {
                    throw new Exception("Error locating email template files: " + ex.Message);
                }                           
                string htmlTemplate = await File.ReadAllTextAsync(templatePath);
                string cssContent = await File.ReadAllTextAsync(cssPath);

                string finalHtml = htmlTemplate
                    .Replace("<link rel=\"stylesheet\" href=\"TwoFactor.css\">", $"<style>{cssContent}</style>")
                    .Replace("@OTP_CODE@", otp);
                bodyBuilder.HtmlBody = finalHtml;
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    emailSettings["SmtpServer"],
                    int.Parse(emailSettings["SmtpPort"]),
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    emailSettings["SenderEmail"],
                    emailSettings["SenderPassword"]);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return;
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending email: " + ex.Message);
            }
        }
    }
}
