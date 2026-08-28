using System;
using System.Threading.Tasks;
using MedicHp.Application.Features.Auth.DTOs;
using MedicHp.Application.Features.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MediatR;
using MedicHp.Application.Features.Auth.Commands.UpdatePushToken;

namespace MedicHp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;
    private readonly IEmailService _emailService;

    public AuthController(IAuthService authService, ICurrentUserService currentUserService, IMediator mediator, IEmailService emailService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _mediator = mediator;
        _emailService = emailService;
    }

    [HttpPost("push-token")]
    [Authorize]
    public async Task<IActionResult> UpdatePushToken([FromBody] UpdatePushTokenCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { success = true, message = "Push token updated." });
    }

    [HttpPost("register/patient")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientDto request)
    {
        var result = await _authService.RegisterPatientAsync(request);
        return Created("", new { success = true, message = "Registration successful. Please verify your email.", data = result });
    }

    [HttpPost("register/doctor")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterDoctor([FromBody] RegisterDoctorDto request)
    {
        var result = await _authService.RegisterDoctorAsync(request);
        return Created("", new { success = true, message = "Registration successful. Please verify your email.", data = result });
    }

    [HttpPost("invite-admin")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> InviteAdmin([FromBody] InviteAdminDto request)
    {
        var result = await _authService.InviteAdminAsync(request);
        return Created("", new { success = true, message = "Admin invited successfully.", data = result });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(new { success = true, message = "Login successful.", data = result });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return Ok(new { success = true, data = result });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto request)
    {
        await _authService.LogoutAsync(request.RefreshToken);
        return Ok(new { success = true, message = "Logged out successfully." });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        await _authService.ForgotPasswordAsync(request);
        return Ok(new { success = true, message = "If an account with that email exists, a password reset link has been sent." });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        await _authService.ResetPasswordAsync(request);
        return Ok(new { success = true, message = "Password has been reset successfully. Please log in." });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        await _authService.ChangePasswordAsync(userId.Value, request);
        return Ok(new { success = true, message = "Password changed successfully." });
    }

    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto request)
    {
        await _authService.VerifyEmailAsync(request);
        return Ok(new { success = true, message = "Email verified successfully." });
    }

    [HttpPost("resend-verification")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto request)
    {
        await _authService.ResendVerificationEmailAsync(request);
        return Ok(new { success = true, message = "Verification code sent to your email." });
    }

    [HttpGet("test-email")]
    [AllowAnonymous]
    public async Task<IActionResult> TestEmail([FromQuery] string to, [FromServices] Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        try
        {
            var pwd = configuration["SMTP_PASSWORD"];
            if (string.IsNullOrEmpty(pwd))
            {
                return BadRequest(new { success = false, error = "SMTP_PASSWORD is NULL or EMPTY in environment variables!" });
            }

            await _emailService.SendWelcomeEmailAsync(to, "Test Diagnostic");
            return Ok(new { success = true, message = $"Email sent successfully! Password length: {pwd.Length}" });
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message, stackTrace = ex.StackTrace, inner = ex.InnerException?.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var profile = await _authService.GetProfileAsync(userId.Value);
        return Ok(new { success = true, data = profile });
    }

    [HttpPut("profile")]
    [Authorize]
    public IActionResult UpdateProfile()
    {
        // Placeholder for future sprint
        return Ok(new { success = true, message = "Profile updated (Not Implemented Yet)" });
    }

    [HttpDelete("account")]
    [Authorize]
    public IActionResult DeleteAccount()
    {
        // Placeholder for future sprint
        return Ok(new { success = true, message = "Account scheduled for deletion (Not Implemented Yet)" });
    }
}
