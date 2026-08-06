using System;
using System.Threading.Tasks;
using MedCore.Application.Features.Auth.DTOs;
using MedCore.Application.Features.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers.v1;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(IAuthService authService, ICurrentUserService currentUserService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
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
