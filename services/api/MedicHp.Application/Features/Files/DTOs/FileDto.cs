using System;

namespace MedicHp.Application.Features.Files.DTOs;

public class FileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string StoragePath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSizeBytes { get; set; }
    public string Purpose { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
