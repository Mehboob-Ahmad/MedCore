using System;
using System.Collections.Generic;

namespace MedicHp.Application.Features.DoctorSearch.DTOs;

public class DoctorSearchResultDto
{
    public Guid DoctorId { get; set; }
    public string FullName { get; set; } = null!;
    public string? ProfilePhotoUrl { get; set; }
    public string? Bio { get; set; }
    public decimal ConsultationFee { get; set; }
    public int YearsOfExperience { get; set; }
    public string? CityName { get; set; }
    public string? ClinicName { get; set; }
    public string? Languages { get; set; }
    public string? Gender { get; set; }
    public List<string> Specializations { get; set; } = new();
    
    // For sorting by relevance (e.g. matched disease/symptom score)
    public int RelevanceScore { get; set; }
}

public class DoctorSearchResponseDto
{
    public List<DoctorSearchResultDto> Doctors { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
