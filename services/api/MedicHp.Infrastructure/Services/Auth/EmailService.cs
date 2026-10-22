using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MedicHp.Application.Features.Auth.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MedicHp.Infrastructure.Services.Auth;

public class EmailService : IEmailService
{
    private const string DefaultSenderEmail = "medcore.pk.official@gmail.com";
    private const string DefaultSenderDisplayName = "MediCore Official";
    private const string DefaultSmtpHost = "smtp.gmail.com";
    private const int DefaultSmtpPort = 587;

    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        // Enforce medcore.pk.official@gmail.com as the permanent sender
        var fromEmail = !string.IsNullOrWhiteSpace(_configuration["SMTP_EMAIL"])
            ? _configuration["SMTP_EMAIL"]!.Trim()
            : DefaultSenderEmail;

        var fromDisplayName = !string.IsNullOrWhiteSpace(_configuration["SMTP_DISPLAY_NAME"])
            ? _configuration["SMTP_DISPLAY_NAME"]!.Trim()
            : DefaultSenderDisplayName;

        var password = _configuration["SMTP_PASSWORD"]?.Replace(" ", "").Trim();

        var host = !string.IsNullOrWhiteSpace(_configuration["SMTP_HOST"])
            ? _configuration["SMTP_HOST"]!.Trim()
            : DefaultSmtpHost;

        var port = int.TryParse(_configuration["SMTP_PORT"], out var p) ? p : DefaultSmtpPort;

        if (string.IsNullOrEmpty(password))
        {
            _logger.LogWarning(
                "[EmailService] SMTP_PASSWORD is not configured. Email to {To} with subject '{Subject}' was not dispatched. Sender: {Sender}",
                to, subject, fromEmail);
            return;
        }

        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromDisplayName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(to.Trim());

            using var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true,
                Timeout = 20000 // 20s timeout
            };

            await smtpClient.SendMailAsync(message);
            _logger.LogInformation("[EmailService] Email successfully sent to {To} from {From} via Google SMTP ({Host}:{Port})",
                to, fromEmail, host, port);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EmailService] Failed to send email to {To} from {From} via Google SMTP: {Message}",
                to, fromEmail, ex.Message);
            throw; // Rethrow to surface exact error to diagnostic endpoint
        }
    }

    public async Task SendVerificationEmailAsync(string to, string otpCode)
    {
        _logger.LogInformation("[EmailService] Sending Verification Email to request generator {To}", to);

        var body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>MediCore Verification Code</title>
</head>
<body style=""margin: 0; padding: 0; background-color: #f8fafc; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color: #1e293b;"">
  <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""background-color: #f8fafc; padding: 40px 16px;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" width=""100%"" style=""max-width: 560px; background-color: #ffffff; border-radius: 16px; border: 1px solid #e2e8f0; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05);"">
          <!-- Header -->
          <tr>
            <td style=""background: linear-gradient(135deg, #0ea5e9 0%, #0284c7 100%); padding: 32px 32px 28px; text-align: center;"">
              <h1 style=""margin: 0; color: #ffffff; font-size: 26px; font-weight: 800; letter-spacing: -0.5px;"">MediCore</h1>
              <p style=""margin: 6px 0 0; color: #e0f2fe; font-size: 14px; font-weight: 500;"">Healthcare & Clinical Management</p>
            </td>
          </tr>

          <!-- Content -->
          <tr>
            <td style=""padding: 36px 32px;"">
              <h2 style=""margin: 0 0 12px; font-size: 20px; font-weight: 700; color: #0f172a;"">Verify Your Email Address</h2>
              <p style=""margin: 0 0 24px; font-size: 15px; line-height: 1.6; color: #475569;"">
                Hello,<br>
                Thank you for using MediCore. Please enter the following One-Time Password (OTP) to verify your account:
              </p>

              <!-- OTP Box -->
              <div style=""background-color: #f0fdf4; border: 2px dashed #16a34a; border-radius: 12px; padding: 24px; text-align: center; margin: 0 0 24px;"">
                <span style=""display: block; font-size: 12px; font-weight: 700; text-transform: uppercase; letter-spacing: 1.5px; color: #15803d; margin-bottom: 8px;"">Your Verification Code</span>
                <span style=""display: inline-block; font-family: 'SF Mono', Consolas, Monaco, monospace; font-size: 38px; font-weight: 800; letter-spacing: 12px; color: #166534; padding-left: 12px;"">{otpCode}</span>
                <p style=""margin: 12px 0 0; font-size: 13px; color: #15803d; font-weight: 500;"">&#9200; Valid for 15 minutes only</p>
              </div>

              <!-- Security Information -->
              <div style=""background-color: #f8fafc; border-radius: 8px; padding: 16px; border-left: 4px solid #0284c7; margin: 0 0 24px;"">
                <p style=""margin: 0; font-size: 13px; line-height: 1.5; color: #334155;"">
                  <strong>Security Reminder:</strong> Never share this code with anyone. MediCore staff will never contact you asking for your verification code.
                </p>
              </div>

              <p style=""margin: 0; font-size: 14px; line-height: 1.5; color: #64748b;"">
                If you did not make this request, you can safely ignore this email. No changes will be made to your account.
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""background-color: #f8fafc; border-top: 1px solid #e2e8f0; padding: 24px 32px; text-align: center;"">
              <p style=""margin: 0 0 6px; font-size: 13px; color: #64748b;"">
                Sent by <strong>MediCore Official</strong> &bull; <a href=""mailto:medcore.pk.official@gmail.com"" style=""color: #0284c7; text-decoration: none;"">medcore.pk.official@gmail.com</a>
              </p>
              <p style=""margin: 0; font-size: 12px; color: #94a3b8;"">
                &copy; {DateTime.UtcNow.Year} MediCore Pakistan. All rights reserved.
              </p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";

        await SendEmailAsync(to, "MediCore — Verify Your Email Code: " + otpCode, body);
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken)
    {
        _logger.LogInformation("[EmailService] Sending Password Reset Email to {To}", to);
        var frontendUrl = _configuration["FRONTEND_URL"] ?? "https://www.medichp.com";
        var resetLink = $"{frontendUrl.TrimEnd('/')}/reset-password?token={resetToken}&email={Uri.EscapeDataString(to)}";

        var body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <title>Reset Your MediCore Password</title>
</head>
<body style=""margin: 0; padding: 0; background-color: #f8fafc; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; color: #1e293b;"">
  <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""padding: 40px 16px;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" width=""100%"" style=""max-width: 560px; background-color: #ffffff; border-radius: 16px; border: 1px solid #e2e8f0; overflow: hidden;"">
          <tr>
            <td style=""background: #0284c7; padding: 28px; text-align: center;"">
              <h1 style=""margin: 0; color: #ffffff; font-size: 24px; font-weight: 800;"">MediCore</h1>
            </td>
          </tr>
          <tr>
            <td style=""padding: 36px 32px;"">
              <h2 style=""margin: 0 0 12px; font-size: 20px; font-weight: 700; color: #0f172a;"">Password Reset Request</h2>
              <p style=""font-size: 15px; line-height: 1.6; color: #475569;"">
                We received a request to reset the password for your MediCore account. Click the button below to choose a new password:
              </p>
              <div style=""text-align: center; margin: 30px 0;"">
                <a href=""{resetLink}"" style=""background-color: #0284c7; color: #ffffff; padding: 14px 28px; border-radius: 8px; font-size: 15px; font-weight: 600; text-decoration: none; display: inline-block;"">Reset Password</a>
              </div>
              <p style=""font-size: 13px; color: #64748b;"">This link will expire in 60 minutes. If you did not request a password reset, you can safely ignore this email.</p>
            </td>
          </tr>
          <tr>
            <td style=""background-color: #f8fafc; border-top: 1px solid #e2e8f0; padding: 20px; text-align: center; font-size: 12px; color: #94a3b8;"">
              Sent by MediCore Official &bull; <a href=""mailto:medcore.pk.official@gmail.com"" style=""color: #0284c7;"">medcore.pk.official@gmail.com</a>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";

        await SendEmailAsync(to, "MediCore — Password Reset Request", body);
    }

    public async Task SendWelcomeEmailAsync(string to, string name, string tempPassword)
    {
        _logger.LogInformation("[EmailService] Sending Welcome Email to {To} for {Name}", to, name);
        var loginUrl = _configuration["FRONTEND_URL"] ?? "https://www.medichp.com/login";

        var body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <title>Welcome to MediCore</title>
</head>
<body style=""margin: 0; padding: 0; background-color: #f8fafc; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; color: #1e293b;"">
  <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""padding: 40px 16px;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" width=""100%"" style=""max-width: 560px; background-color: #ffffff; border-radius: 16px; border: 1px solid #e2e8f0; overflow: hidden;"">
          <tr>
            <td style=""background: #0284c7; padding: 28px; text-align: center;"">
              <h1 style=""margin: 0; color: #ffffff; font-size: 24px; font-weight: 800;"">MediCore</h1>
            </td>
          </tr>
          <tr>
            <td style=""padding: 36px 32px;"">
              <h2 style=""margin: 0 0 12px; font-size: 20px; font-weight: 700; color: #0f172a;"">Welcome to MediCore!</h2>
              <p style=""font-size: 15px; line-height: 1.6; color: #475569;"">
                Hello {name},<br>
                Your practitioner account on MediCore has been created. Here are your temporary login credentials:
              </p>
              <div style=""background-color: #f1f5f9; border-radius: 8px; padding: 18px; margin: 20px 0;"">
                <p style=""margin: 0 0 8px; font-size: 14px;""><strong>Email:</strong> {to}</p>
                <p style=""margin: 0; font-size: 14px;""><strong>Temporary Password:</strong> <code style=""background: #ffffff; padding: 2px 6px; border-radius: 4px; border: 1px solid #cbd5e1;"">{tempPassword}</code></p>
              </div>
              <p style=""font-size: 14px; color: #e11d48; font-weight: 500;"">Please log in and update your password immediately for security.</p>
              <div style=""text-align: center; margin: 26px 0;"">
                <a href=""{loginUrl}"" style=""background-color: #0284c7; color: #ffffff; padding: 12px 24px; border-radius: 8px; font-size: 15px; font-weight: 600; text-decoration: none; display: inline-block;"">Log In to MediCore</a>
              </div>
            </td>
          </tr>
          <tr>
            <td style=""background-color: #f8fafc; border-top: 1px solid #e2e8f0; padding: 20px; text-align: center; font-size: 12px; color: #94a3b8;"">
              Sent by MediCore Official &bull; <a href=""mailto:medcore.pk.official@gmail.com"" style=""color: #0284c7;"">medcore.pk.official@gmail.com</a>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";

        await SendEmailAsync(to, "Welcome to MediCore — Account Credentials", body);
    }

    public async Task SendAppointmentRescheduledEmailAsync(string to, string dateStr)
    {
        _logger.LogInformation("[EmailService] Sending Appointment Rescheduled Email to {To}", to);
        var body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head><meta charset=""utf-8""><title>Appointment Rescheduled</title></head>
<body style=""margin: 0; padding: 0; background-color: #f8fafc; font-family: sans-serif; color: #1e293b;"">
  <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""padding: 40px 16px;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" width=""100%"" style=""max-width: 560px; background-color: #ffffff; border-radius: 16px; border: 1px solid #e2e8f0; padding: 32px;"">
          <h2 style=""color: #0284c7; margin-top: 0;"">Appointment Rescheduled</h2>
          <p style=""font-size: 15px; line-height: 1.6;"">Your appointment has been successfully rescheduled to: <strong>{dateStr}</strong></p>
          <p style=""font-size: 14px; color: #64748b;"">Your original payment is valid for this new slot.</p>
          <hr style=""border: none; border-top: 1px solid #e2e8f0; margin: 24px 0;"" />
          <p style=""font-size: 12px; color: #94a3b8; text-align: center; margin: 0;"">MediCore Official &bull; medcore.pk.official@gmail.com</p>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";
        await SendEmailAsync(to, "MediCore — Appointment Rescheduled", body);
    }

    public async Task SendDemoRequestAdminNotificationAsync(string adminEmail, string applicantName, string specialization, string email, string phone, string clinic)
    {
        _logger.LogInformation("[EmailService] Sending Demo Request Admin Notification to {To}", adminEmail);
        var body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head><meta charset=""utf-8""><title>New Doctor Demo Request</title></head>
<body style=""margin: 0; padding: 0; background-color: #f8fafc; font-family: sans-serif; color: #1e293b;"">
  <table role=""presentation"" width=""100%"" style=""padding: 30px 16px;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" width=""100%"" style=""max-width: 560px; background: #ffffff; border-radius: 12px; border: 1px solid #e2e8f0; padding: 28px;"">
          <h2 style=""color: #0284c7; margin-top: 0;"">New Doctor Demo Request</h2>
          <p style=""font-size: 14px; color: #475569;"">A new medical professional has requested a demo of MediCore:</p>
          <table style=""width: 100%; border-collapse: collapse; font-size: 14px; margin: 16px 0;"">
            <tr><td style=""padding: 8px 0; font-weight: bold; width: 140px;"">Name:</td><td>{applicantName}</td></tr>
            <tr><td style=""padding: 8px 0; font-weight: bold;"">Specialization:</td><td>{specialization}</td></tr>
            <tr><td style=""padding: 8px 0; font-weight: bold;"">Email:</td><td><a href=""mailto:{email}"">{email}</a></td></tr>
            <tr><td style=""padding: 8px 0; font-weight: bold;"">Phone:</td><td>{phone}</td></tr>
            <tr><td style=""padding: 8px 0; font-weight: bold;"">Clinic/Hospital:</td><td>{clinic}</td></tr>
          </table>
          <p style=""font-size: 13px; color: #64748b;"">Log into the MediCore Admin Portal to review and approve this practitioner.</p>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";
        await SendEmailAsync(adminEmail, "New MediCore Doctor Demo Request: " + applicantName, body);
    }

    public async Task SendDemoRequestApplicantConfirmationAsync(string to, string applicantName)
    {
        _logger.LogInformation("[EmailService] Sending Demo Request Confirmation to request generator {To}", to);
        var body = $@"
<!DOCTYPE html>
<html lang=""en"">
<head><meta charset=""utf-8""><title>Demo Request Received - MediCore</title></head>
<body style=""margin: 0; padding: 0; background-color: #f8fafc; font-family: sans-serif; color: #1e293b;"">
  <table role=""presentation"" width=""100%"" style=""padding: 30px 16px;"">
    <tr>
      <td align=""center"">
        <table role=""presentation"" width=""100%"" style=""max-width: 560px; background: #ffffff; border-radius: 12px; border: 1px solid #e2e8f0; padding: 28px;"">
          <h2 style=""color: #0284c7; margin-top: 0;"">Demo Request Received</h2>
          <p style=""font-size: 15px; line-height: 1.6; color: #334155;"">
            Dear Dr. {applicantName},<br><br>
            Thank you for your interest in <strong>MediCore</strong>. We have received your demo request and credentials.
          </p>
          <p style=""font-size: 14px; line-height: 1.6; color: #475569;"">
            Our onboarding team is reviewing your details and will activate your trial portal shortly. If you have any questions, reach us directly at <a href=""mailto:medcore.pk.official@gmail.com"" style=""color: #0284c7;"">medcore.pk.official@gmail.com</a>.
          </p>
          <hr style=""border: none; border-top: 1px solid #e2e8f0; margin: 24px 0;"" />
          <p style=""font-size: 12px; color: #94a3b8; text-align: center; margin: 0;"">
            &copy; {DateTime.UtcNow.Year} MediCore Official &bull; medcore.pk.official@gmail.com
          </p>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";
        await SendEmailAsync(to, "MediCore — Demo Request Received", body);
    }
}
