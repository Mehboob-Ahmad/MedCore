using System.Threading.Tasks;

namespace MedCore.Application.Features.Auth.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string to, string otpCode);
    Task SendPasswordResetEmailAsync(string to, string resetToken);
    Task SendWelcomeEmailAsync(string to, string name);
}
