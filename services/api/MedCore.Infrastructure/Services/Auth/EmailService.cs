using System.Threading.Tasks;
using MedCore.Application.Features.Auth.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedCore.Infrastructure.Services.Auth;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendVerificationEmailAsync(string to, string otpCode)
    {
        _logger.LogInformation("[EmailService] Sending Verification Email to {To} with OTP {OtpCode}", to, otpCode);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string to, string resetToken)
    {
        _logger.LogInformation("[EmailService] Sending Password Reset Email to {To} with Token {Token}", to, resetToken);
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string to, string name)
    {
        _logger.LogInformation("[EmailService] Sending Welcome Email to {To} for {Name}", to, name);
        return Task.CompletedTask;
    }
}
