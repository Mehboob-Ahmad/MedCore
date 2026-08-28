using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MedicHp.Application.Features.Auth.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MedicHp.Infrastructure.Services.Auth;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = _configuration["SMTP_EMAIL"] ?? "medcore.pk.official@gmail.com";
        var password = _configuration["SMTP_PASSWORD"]; // Must be a Gmail App Password

        if (string.IsNullOrEmpty(password))
        {
            _logger.LogWarning("[EmailService] SMTP_PASSWORD is not configured. Email to {To} was not sent.", to);
            return;
        }

        try
        {
            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(email, password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(email, "MedicHp Admin"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("[EmailService] Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EmailService] Failed to send email to {To}", to);
            throw; // Rethrow to surface exact SMTP error to diagnostic endpoint
        }
    }

    public async Task SendVerificationEmailAsync(string to, string otpCode)
    {
        _logger.LogInformation("[EmailService] Sending Verification Email to {To} with OTP {OtpCode}", to, otpCode);
        await SendEmailAsync(to, "MedicHp - Verify Your Email", $"Your verification code is: <b>{otpCode}</b>");
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken)
    {
        _logger.LogInformation("[EmailService] Sending Password Reset Email to {To} with Token {Token}", to, resetToken);
        await SendEmailAsync(to, "MedicHp - Password Reset", $"Your password reset token is: <b>{resetToken}</b>");
    }

    public async Task SendWelcomeEmailAsync(string to, string name)
    {
        _logger.LogInformation("[EmailService] Sending Welcome Email to {To} for {Name}", to, name);
        string body = $@"
            <h3>Welcome to MedicHp!</h3>
            <p>You have been invited as a {name}.</p>
            <p>Your temporary password is: <b>admin123</b></p>
            <p>Please log in at <a href='https://med-core-web.vercel.app'>MedicHp Portal</a> and change your password immediately.</p>
        ";
        await SendEmailAsync(to, "You are invited to MedicHp", body);
    }
}
