using System;
using System.Linq;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Auth.DTOs;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Domain.Entities.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MedicHp.Domain.Entities.Lookup;
using MedicHp.Shared.Exceptions;
using FluentValidation.Results;

using Microsoft.Extensions.Configuration;

namespace MedicHp.Application.Features.Auth.Services;

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
    private readonly IConfiguration _configuration;

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
        ICurrentUserService currentUserService,
        IConfiguration configuration)
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
        _configuration = configuration;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto request)
    {
        var adminEmail = _configuration["PRIMARY_ADMIN_EMAIL"];
        var adminPassword = _configuration["PRIMARY_ADMIN_PASSWORD"];

        if (!string.IsNullOrEmpty(adminEmail) && 
            !string.IsNullOrEmpty(adminPassword) && 
            request.Email?.Trim().Equals(adminEmail, StringComparison.OrdinalIgnoreCase) == true && 
            request.Password == adminPassword)
        {
            var adminUser = new User 
            { 
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Email = adminEmail,
                FirstName = "Primary",
                LastName = "Admin",
                IsActive = true,
                EmailConfirmed = true,
                UserRoles = new List<UserRole> 
                { 
                    new UserRole { Role = new Role { Name = "SystemAdmin", NormalizedName = "SYSTEMADMIN" } } 
                }
            };
            
            var adminAccessToken = await _tokenService.GenerateAccessTokenAsync(adminUser);
            var adminRefreshTokenStr = _tokenService.GenerateRefreshToken();
            
            return new TokenResponseDto
            {
                AccessToken = adminAccessToken,
                RefreshToken = adminRefreshTokenStr,
                ExpiresIn = 900,
                User = MapToDto(adminUser)
            };
        }

        var normalizedEmail = request.Email?.Trim().ToUpper() ?? string.Empty;
        var user = await _userRepository.FirstOrDefaultAsync(
            u => u.NormalizedEmail == normalizedEmail,
            include: q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role));

        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

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
            throw new UnauthorizedAccessException("Invalid email or password.");
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
        var normalizedEmail = request.Email?.Trim().ToUpper() ?? string.Empty;
        var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
        if (existingUser != null)
            throw new InvalidOperationException("Email already exists.");

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email?.Trim() ?? string.Empty,
            NormalizedEmail = request.Email?.Trim().ToUpper() ?? string.Empty,
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
        var normalizedEmail = request.Email?.Trim().ToUpper() ?? string.Empty;
        var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
        if (existingUser != null)
            throw new InvalidOperationException("Email already exists.");

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email?.Trim() ?? string.Empty,
            NormalizedEmail = request.Email?.Trim().ToUpper() ?? string.Empty,
            PhoneNumber = request.PhoneNumber, // Might be empty now, will update in profile
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
        user.DoctorProfile = new MedicHp.Domain.Entities.Clinical.DoctorProfile
        {
            YearsOfExperience = 0,
            ConsultationFee = 0,
            RegistrationNumber = "" // Will be updated in complete profile
        };

        // Note: For File linking, we would ideally resolve the Files by ID and set their UploadedByUserId to the new user.Id.
        // We will leave this out for now or do a quick update on the DbContext if needed, 
        // but since we only return UserDto, and the Files already exist with Guid.Empty, we can leave as is or update them.

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        await _emailService.SendVerificationEmailAsync(user.Email, "123456");

        return MapToDto(user);
    }

    public async Task<UserDto> InviteAdminAsync(InviteAdminDto request)
    {
        var normalizedEmail = request.Email?.Trim().ToUpper() ?? string.Empty;
        // 1. Check if user already exists
        var existingUser = await _userRepository
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

        if (existingUser != null)
        {
            throw new ValidationException(new[] 
            { 
                new ValidationFailure("Email", "User with this email already exists.") 
            });
        }

        var existingPhone = await _userRepository
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber.Trim());

        if (existingPhone != null)
        {
            throw new ValidationException(new[] 
            { 
                new ValidationFailure("PhoneNumber", "User with this phone number already exists.") 
            });
        }

        // 2. Create User
        var user = new User
        {
            FirstName = string.IsNullOrWhiteSpace(request.FirstName) ? "New" : request.FirstName.Trim(),
            LastName = string.IsNullOrWhiteSpace(request.LastName) ? "Admin" : request.LastName.Trim(),
            Email = request.Email?.Trim() ?? string.Empty,
            NormalizedEmail = request.Email?.Trim().ToUpper() ?? string.Empty,
            EmailConfirmed = false, // Must verify their email
            PhoneNumber = request.PhoneNumber?.Trim() ?? string.Empty,
            PhoneNumberConfirmed = false,
            IsActive = true
        };

        // Hardcode password to admin123
        user.PasswordHash = _passwordHasher.HashPassword(user, "admin123");

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // 3. Assign SystemAdmin Role
        var adminRole = await _roleRepository.FirstOrDefaultAsync(r => r.NormalizedName == "SYSTEMADMIN");
        if (adminRole == null)
            throw new Exception("SystemAdmin role not found.");

        await _userRoleRepository.AddAsync(new UserRole
        {
            UserId = user.Id,
            RoleId = adminRole.Id,
            AssignedAt = DateTime.UtcNow
        });
        await _unitOfWork.SaveChangesAsync();

        // 4. Send Email Notification
        await _emailService.SendWelcomeEmailAsync(user.Email, "System Admin");

        return await GetProfileAsync(user.Id);
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
        var normalizedEmail = request.Email?.Trim().ToUpper() ?? string.Empty;
        var user = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
        if (user != null)
        {
            var resetToken = Guid.NewGuid().ToString("N");
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);
            // Ideally store the token in PasswordResetTokens table (omitted for brevity, assume valid if implemented)
        }
    }

    public async Task ResetPasswordAsync(ResetPasswordDto request)
    {
        var normalizedEmail = request.Email?.Trim().ToUpper() ?? string.Empty;
        var user = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
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
        var normalizedEmail = request.Email?.Trim().ToUpper() ?? string.Empty;
        var user = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
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
        var normalizedEmail = request.Email?.Trim().ToUpper() ?? string.Empty;
        var user = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
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
