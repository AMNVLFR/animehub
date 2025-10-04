using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace AnimeHub.Helpers
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("Smtp");

                using var client = new SmtpClient
                {
                    Host = smtpSettings["Host"],
                    Port = int.Parse(smtpSettings["Port"]),
                    EnableSsl = true,
                    Credentials = new NetworkCredential(
                        smtpSettings["Username"],
                        smtpSettings["Password"]
                    )
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings["FromEmail"], smtpSettings["FromName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log the error - in production you'd want proper logging
                Console.WriteLine($"Email sending failed: {ex.Message}");
                // For development, you might want to fall back to showing the link
                throw;
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var subject = "Reset Your AnimeHub Password";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #333;'>Reset Your Password</h2>
                    <p>Hello,</p>
                    <p>You recently requested to reset your password for your AnimeHub account.</p>
                    <p>Please click the link below to reset your password:</p>
                    <p style='margin: 30px 0;'>
                        <a href='{resetLink}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>Reset Password</a>
                    </p>
                    <p>If you didn't request this password reset, please ignore this email.</p>
                    <p>This link will expire in 24 hours for security reasons.</p>
                    <p>Best regards,<br>The AnimeHub Team</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                    <p style='font-size: 12px; color: #666;'>
                        If the button doesn't work, copy and paste this link into your browser:<br>
                        <a href='{resetLink}'>{resetLink}</a>
                    </p>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}