namespace MedicHp.Application.Features.Admin.DTOs;

public class SystemStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public int MonthlyActive { get; set; }
}
