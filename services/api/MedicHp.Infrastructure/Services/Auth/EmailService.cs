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
        // Try to read SMTP_EMAIL, fallback to RESEND_FROM_EMAIL to preserve old config if they didn't update it yet
        var fromEmail = _configuration["SMTP_EMAIL"] ?? _configuration["RESEND_FROM_EMAIL"];
        var password = _configuration["SMTP_PASSWORD"]; // Google App Password

        if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(password))
        {
            _logger.LogWarning("[EmailService] SMTP_EMAIL or SMTP_PASSWORD is not configured. Email to {To} was not sent.", to);
            return;
        }

        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, "MedicHp Admin"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(to);

            using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(message);
            _logger.LogInformation("[EmailService] Email sent successfully via Gmail SMTP to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EmailService] Failed to send email to {To} via Gmail SMTP", to);
            throw; // Rethrow to surface exact error to diagnostic endpoint
        }
    }

    public async Task SendVerificationEmailAsync(string to, string otpCode)
    {
        _logger.LogInformation("[EmailService] Sending Verification Email to {To} with OTP {OtpCode}", to, otpCode);
        await SendEmailAsync(to, "MedicHp - Verify Your Email", $"Your verification code is: <b>{otpCode}</b>");
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken)
    {
        _logger.LogInformation("[EmailService] Sending Password Reset Email to {To}", to);
        var frontendUrl = _configuration["FRONTEND_URL"] ?? "http://localhost:3000";
        var resetLink = $"{frontendUrl}/reset-password?token={resetToken}&email={Uri.EscapeDataString(to)}";
        
        string body = $@"
            <h3>MedicHp — Password Reset Request</h3>
            <p>Hello,</p>
            <p>A password reset was requested for your MedicHp account.</p>
            <p>Click the link below to create a new password:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            <p>This link will expire in 60 minutes.</p>
            <p>If you did not request a password reset, you can safely ignore this email.</p>
            <p>Regards,<br>MedicHp</p>
        ";
        await SendEmailAsync(to, "MedicHp - Password Reset Request", body);
    }

    public async Task SendWelcomeEmailAsync(string to, string name, string tempPassword)
    {
        _logger.LogInformation("[EmailService] Sending Welcome Email to {To} for {Name}", to, name);
        string body = $@"
            <h3>Welcome to MedicHp!</h3>
            <p>You have been invited as a {name}.</p>
            <p>Your temporary password is: <b>{tempPassword}</b></p>
            <p>Please log in at <a href='https://app.medichp.com/login'>MedicHp Portal</a> and change your password immediately.</p>
        ";
        await SendEmailAsync(to, "You are invited to MedicHp", body);
    }

    public async Task SendAppointmentRescheduledEmailAsync(string to, string dateStr)
    {
        _logger.LogInformation("[EmailService] Sending Appointment Rescheduled Email to {To}", to);
        string body = $@"
            <h3>Appointment Rescheduled</h3>
            <p>Hello,</p>
            <p>Your appointment has been successfully rescheduled to: <b>{dateStr}</b></p>
            <p>Your original payment (if applicable) is still valid for this new slot.</p>
            <p>Regards,<br>MedicHp</p>
        ";
        await SendEmailAsync(to, "MedicHp - Appointment Rescheduled", body);
    }
}
