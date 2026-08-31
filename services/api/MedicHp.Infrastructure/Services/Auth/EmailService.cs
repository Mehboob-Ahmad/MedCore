using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MedicHp.Application.Features.Auth.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MedicHp.Infrastructure.Services.Auth;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration, HttpClient httpClient)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.resend.com/");
    }

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        var apiKey = _configuration["RESEND_API_KEY"];
        var fromEmail = _configuration["RESEND_FROM_EMAIL"] ?? "onboarding@resend.dev"; // Default for unverified domains

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("[EmailService] RESEND_API_KEY is not configured. Email to {To} was not sent.", to);
            return;
        }

        try
        {
            var payload = new
            {
                from = $"MedicHp Admin <{fromEmail}>",
                to = new[] { to },
                subject = subject,
                html = body
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "emails")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("[EmailService] Email sent successfully via Resend to {To}", to);
            }
            else
            {
                _logger.LogError("[EmailService] Resend API failed: {StatusCode} - {Body}", response.StatusCode, responseBody);
                throw new Exception($"Resend API error: {responseBody}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EmailService] Failed to send email to {To}", to);
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
}
