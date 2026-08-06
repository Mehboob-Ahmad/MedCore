using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediatR;

namespace MedCore.Application.Features.Doctors.Commands.ConfigureAvailability;

public class ConfigureAvailabilityCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    
    public List<AvailabilityDayDto> Days { get; set; } = new();
}

public class AvailabilityDayDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty; // HH:mm
    public string EndTime { get; set; } = string.Empty; // HH:mm
    public bool IsAvailable { get; set; }
}
