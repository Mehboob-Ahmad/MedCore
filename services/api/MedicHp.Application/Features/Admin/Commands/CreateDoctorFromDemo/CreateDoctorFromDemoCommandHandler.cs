using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Domain.Entities.Admin;
using MedicHp.Domain.Entities.Core;
using MedicHp.Domain.Entities.Lookup;
using MedicHp.Domain.Entities.Clinical;
using MedicHp.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MedicHp.Application.Features.Admin.Commands.CreateDoctorFromDemo;

public class CreateDoctorFromDemoCommandHandler : IRequestHandler<CreateDoctorFromDemoCommand, bool>
{
    private readonly IGenericRepository<DemoRequest> _demoRequestRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDoctorFromDemoCommandHandler(
        IGenericRepository<DemoRequest> demoRequestRepository,
        IGenericRepository<User> userRepository,
        IGenericRepository<Role> roleRepository,
        IPasswordHasher<User> passwordHasher,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _demoRequestRepository = demoRequestRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CreateDoctorFromDemoCommand request, CancellationToken cancellationToken)
    {
        var demoRequest = await _demoRequestRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (demoRequest == null)
            throw new NotFoundException(nameof(DemoRequest), request.RequestId);

        if (demoRequest.Status == DemoRequestStatus.ConvertedToProduction || demoRequest.Status == DemoRequestStatus.DemoCreated)
            throw new InvalidOperationException("This demo request has already been converted to an account.");

        // Check if email already exists
        var normalizedEmail = demoRequest.Email.Trim().ToUpper();
        var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
        if (existingUser != null)
            throw new InvalidOperationException("A user with this email already exists.");

        // Split FullName
        var nameParts = demoRequest.FullName.Split(' ', 2);
        var firstName = nameParts.Length > 0 ? nameParts[0] : "Doctor";
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";

        // Create User
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = demoRequest.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            PhoneNumber = demoRequest.PhoneNumber,
            AccountStatus = MedicHp.Domain.Enums.AccountStatus.Active,
            EmailConfirmed = true // verified by admin
        };

        // Generate temporary password
        var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8) + "D1!";
        user.PasswordHash = _passwordHasher.HashPassword(user, tempPassword);

        // Assign Doctor Role
        var doctorRole = await _roleRepository.FirstOrDefaultAsync(r => r.NormalizedName == "DOCTOR");
        if (doctorRole != null)
        {
            user.UserRoles.Add(new UserRole { RoleId = doctorRole.Id });
        }

        // Create Doctor Profile
        user.DoctorProfile = new DoctorProfile
        {
            Specialization = demoRequest.Specialization,
            YearsOfExperience = demoRequest.YearsOfExperience,
            ClinicName = demoRequest.ClinicOrHospital,
            IsDemoAccount = false,
            VerificationDocumentUrl = demoRequest.DegreeImageUrl ?? demoRequest.LicenseImageUrl,
            VerificationStatus = "Approved"
        };

        await _userRepository.AddAsync(user);

        // Update Demo Request
        demoRequest.Status = DemoRequestStatus.ConvertedToProduction;
        demoRequest.UpdatedAt = DateTime.UtcNow;
        await _demoRequestRepository.UpdateAsync(demoRequest, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send Welcome Email
        await _emailService.SendWelcomeEmailAsync(user.Email, $"{firstName} {lastName}", tempPassword);

        return true;
    }
}
