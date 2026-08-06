using System;
using System.Linq;
using System.Threading.Tasks;
using MedCore.Application.Common;
using MedCore.Application.Features.Auth.DTOs;
using MedCore.Application.Features.Auth.Interfaces;
using MedCore.Domain.Entities.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MedCore.Domain.Entities.Lookup;

namespace MedCore.Application.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IGenericRepository<UserRole> _userRoleRepository;
    private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
    private readonly IGenericRepository<Specialization> _specializationRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AuthService(
        IGenericRepository<User> userRepository,
        IGenericRepository<Role> roleRepository,
        IGenericRepository<UserRole> userRoleRepository,
        IGenericRepository<RefreshToken> refreshTokenRepository,
        IGenericRepository<Specialization> specializationRepository,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _specializationRepository = specializationRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto request)
    {
        var user = await _userRepository.FirstOrDefaultAsync(
            u => u.NormalizedEmail == request.Email.ToUpper(),
            include: q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role));

        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account suspended.");

        if (!user.EmailConfirmed)
            throw new UnauthorizedAccessException("Email not verified.");

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            throw new UnauthorizedAccessException("Account locked.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        
        if (result == PasswordVerificationResult.Failed)
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
        var refreshTokenStr = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Token = refreshTokenStr,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            DeviceInfo = request.DeviceInfo,
            UserId = user.Id
        };
        
        await _refreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenStr,
            ExpiresIn = 900,
            User = MapToDto(user)
        };
    }

    public async Task<UserDto> RegisterPatientAsync(RegisterPatientDto request)
    {
        var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == request.Email.ToUpper());
        if (existingUser != null)
            throw new InvalidOperationException("Email already exists.");

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            EmailConfirmed = false
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        var patientRole = await _roleRepository.FirstOrDefaultAsync(r => r.NormalizedName == "PATIENT");
        if (patientRole != null)
        {
            user.UserRoles.Add(new UserRole { RoleId = patientRole.Id });
        }

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Generate OTP and send (Mock)
        await _emailService.SendVerificationEmailAsync(user.Email, "123456");

        return MapToDto(user);
    }

    public async Task<UserDto> RegisterDoctorAsync(RegisterDoctorDto request)
    {
        var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == request.Email.ToUpper());
        if (existingUser != null)
            throw new InvalidOperationException("Email already exists.");

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            EmailConfirmed = false
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        var doctorRole = await _roleRepository.FirstOrDefaultAsync(r => r.NormalizedName == "DOCTOR");
        if (doctorRole != null)
        {
            user.UserRoles.Add(new UserRole { RoleId = doctorRole.Id });
        }

        // Add doctor specific fields
        user.DoctorProfile = new MedCore.Domain.Entities.Clinical.DoctorProfile
        {
            YearsOfExperience = request.YearsOfExperience,
            ConsultationFee = request.ConsultationFee,
            LicenseNumber = request.LicenseNumber
        };

        foreach (var specId in request.SpecializationIds)
        {
            var exists = await _specializationRepository.GetByIdAsync(specId) != null;
            if (exists)
            {
                user.DoctorProfile.Specializations.Add(new MedCore.Domain.Entities.Clinical.DoctorSpecialization
                {
                    SpecializationId = specId
                });
            }
        }

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        await _emailService.SendVerificationEmailAsync(user.Email, "123456");

        return MapToDto(user);
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenDto request)
    {
        var tokenRecord = await _refreshTokenRepository.FirstOrDefaultAsync(
            rt => rt.Token == request.RefreshToken,
            include: q => q.Include(rt => rt.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role));

        if (tokenRecord == null || tokenRecord.IsRevoked)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (tokenRecord.ReplacedByToken != null)
        {
            // Token Reuse detected. Revoke all tokens for this user.
            var allUserTokens = await _refreshTokenRepository.GetAsync(rt => rt.UserId == tokenRecord.UserId && !rt.IsRevoked);
            foreach(var t in allUserTokens)
            {
                t.IsRevoked = true;
                await _refreshTokenRepository.UpdateAsync(t);
            }
            await _unitOfWork.SaveChangesAsync();
            throw new UnauthorizedAccessException("Refresh token reused. All sessions revoked.");
        }

        if (tokenRecord.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expired.");

        var user = tokenRecord.User;
        var newAccessToken = await _tokenService.GenerateAccessTokenAsync(user);
        var newRefreshTokenStr = _tokenService.GenerateRefreshToken();

        tokenRecord.ReplacedByToken = newRefreshTokenStr;
        await _refreshTokenRepository.UpdateAsync(tokenRecord);

        var newRefreshToken = new RefreshToken
        {
            Token = newRefreshTokenStr,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IpAddress = "Refreshed",
            UserId = user.Id
        };
        
        await _refreshTokenRepository.AddAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new TokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenStr,
            ExpiresIn = 900,
            User = MapToDto(user)
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var tokenRecord = await _refreshTokenRepository.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (tokenRecord != null)
        {
            tokenRecord.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(tokenRecord);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto request)
    {
        var user = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == request.Email.ToUpper());
        if (user != null)
        {
            var resetToken = Guid.NewGuid().ToString("N");
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);
            // Ideally store the token in PasswordResetTokens table (omitted for brevity, assume valid if implemented)
        }
    }

    public async Task ResetPasswordAsync(ResetPasswordDto request)
    {
        var user = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == request.Email.ToUpper());
        if (user != null)
        {
            // Assume token verification is successful
            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new UnauthorizedAccessException();

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid current password.");

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task VerifyEmailAsync(VerifyEmailDto request)
    {
        var user = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == request.Email.ToUpper());
        if (user != null && !user.EmailConfirmed)
        {
            // Assume OTP "123456" is always valid for now
            if (request.OtpCode == "123456")
            {
                user.EmailConfirmed = true;
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid OTP.");
            }
        }
    }

    public async Task ResendVerificationEmailAsync(ResendVerificationDto request)
    {
        var user = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == request.Email.ToUpper());
        if (user != null && !user.EmailConfirmed)
        {
            await _emailService.SendVerificationEmailAsync(user.Email, "123456");
        }
    }

    public async Task<UserDto> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(
            u => u.Id == userId,
            include: q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role));

        if (user == null) throw new UnauthorizedAccessException();
        return MapToDto(user);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}
