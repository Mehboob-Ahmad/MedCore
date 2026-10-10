using System.Threading.Tasks;

namespace MedicHp.Application.Features.Auth.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string to, string otpCode);
    Task SendPasswordResetEmailAsync(string to, string resetToken);
    Task SendWelcomeEmailAsync(string to, string name, string tempPassword);
    Task SendAppointmentRescheduledEmailAsync(string to, string dateStr);
    Task SendDemoRequestAdminNotificationAsync(string adminEmail, string applicantName, string specialization, string email, string phone, string clinic);
    Task SendDemoRequestApplicantConfirmationAsync(string to, string applicantName);
}
