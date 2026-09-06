using System;
using System.Collections.Generic;
using MediatR;
using MedicHp.Domain.Entities.Admin;

namespace MedicHp.Application.Features.Admin.Queries.GetDemoRequests;

public class DemoRequestDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ClinicOrHospital { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string ProfessionalQualification { get; set; } = string.Empty;
    public DemoRequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetDemoRequestsQuery : IRequest<List<DemoRequestDto>>
{
    public DemoRequestStatus? Status { get; set; }
}
