using System;
using System.Threading.Tasks;
using MedCore.Application.Features.Auth.DTOs;

namespace MedCore.Application.Features.Auth.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginDto request);
    Task<UserDto> RegisterPatientAsync(RegisterPatientDto request);
    Task<UserDto> RegisterDoctorAsync(RegisterDoctorDto request);
    Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenDto request);
    Task LogoutAsync(string refreshToken);
    
    Task ForgotPasswordAsync(ForgotPasswordDto request);
    Task ResetPasswordAsync(ResetPasswordDto request);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto request);
    
    Task VerifyEmailAsync(VerifyEmailDto request);
    Task ResendVerificationEmailAsync(ResendVerificationDto request);
    
    Task<UserDto> GetProfileAsync(Guid userId);
}
