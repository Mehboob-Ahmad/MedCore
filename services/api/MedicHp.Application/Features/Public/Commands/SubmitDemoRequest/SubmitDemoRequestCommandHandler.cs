using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Domain.Entities.Admin;
using MediatR;
using MedicHp.Application.Features.Auth.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicHp.Application.Features.Public.Commands.SubmitDemoRequest;

public class SubmitDemoRequestCommandHandler : IRequestHandler<SubmitDemoRequestCommand, Guid>
{
    private readonly IGenericRepository<DemoRequest> _demoRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubmitDemoRequestCommandHandler> _logger;
    private readonly IConfiguration _configuration;

    public SubmitDemoRequestCommandHandler(
        IGenericRepository<DemoRequest> demoRequestRepository, 
        IUnitOfWork unitOfWork,
        IServiceProvider serviceProvider,
        ILogger<SubmitDemoRequestCommandHandler> logger,
        IConfiguration configuration)
    {
        _demoRequestRepository = demoRequestRepository;
        _unitOfWork = unitOfWork;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<Guid> Handle(SubmitDemoRequestCommand request, CancellationToken cancellationToken)
    {
        var demoRequest = new DemoRequest
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            ProfessionalType = request.ProfessionalType,
            Specialization = request.Specialization,
            City = request.City,
            ClinicOrHospital = request.ClinicOrHospital,
            YearsOfExperience = request.YearsOfExperience,
            ProfessionalQualification = request.ProfessionalQualification,
            AdditionalInformation = request.AdditionalInformation,
            DegreeImageUrl = request.DegreeImageUrl,
            LicenseImageUrl = request.LicenseImageUrl,
            Status = DemoRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _demoRequestRepository.AddAsync(demoRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Safely send emails without blocking or failing the request
        try
        {
            var adminEmail = _configuration["ADMIN_EMAIL"] ?? "admin@medichp.com";
            
            // Fire and forget using Task.Run to not block the response and avoid 500 on email failure
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    await emailService.SendDemoRequestAdminNotificationAsync(
                        adminEmail, 
                        demoRequest.FullName, 
                        demoRequest.Specialization, 
                        demoRequest.Email, 
                        demoRequest.PhoneNumber, 
                        demoRequest.ClinicOrHospital);
                    
                    await emailService.SendDemoRequestApplicantConfirmationAsync(
                        demoRequest.Email, 
                        demoRequest.FullName);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Background email sending failed for DemoRequest {DemoRequestId}", demoRequest.Id);
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule emails for DemoRequest {DemoRequestId}", demoRequest.Id);
        }

        return demoRequest.Id;
    }
}
