using System;
using MedCore.Application.Features.DoctorSearch.DTOs;
using MediatR;

namespace MedCore.Application.Features.DoctorSearch.Queries.SearchDoctors;

public class SearchDoctorsQuery : IRequest<DoctorSearchResponseDto>
{
    public string? SearchTerm { get; set; }
    public string? Disease { get; set; }
    public string? Symptom { get; set; }
    public string? Specialization { get; set; }
    public Guid? CityId { get; set; }
    public string? ClinicName { get; set; }
    public string? Language { get; set; }
    public decimal? MaxFee { get; set; }
    public int? MinExperienceYears { get; set; }
    public string? Gender { get; set; }
    
    public string? SortBy { get; set; } // Relevance, LowestFee, HighestExperience, Newest, Alphabetical
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
