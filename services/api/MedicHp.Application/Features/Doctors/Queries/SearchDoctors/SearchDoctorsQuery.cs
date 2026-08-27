using MedicHp.Application.Features.Doctors.DTOs;
using MediatR;
using System.Collections.Generic;

namespace MedicHp.Application.Features.Doctors.Queries.SearchDoctors;

public class SearchDoctorsQuery : IRequest<List<DoctorSearchDto>>
{
    public string? SearchTerm { get; set; }
    public string? Specialty { get; set; }
    public string? Gender { get; set; }
    public int? MaxDistanceKm { get; set; } // Future use
    
    // Pagination could be added here
}
