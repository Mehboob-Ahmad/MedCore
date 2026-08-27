using System;
using MedicHp.Domain.Common;

namespace MedicHp.Domain.Entities.Core;

public class File : SoftDeleteEntity
{
    public Guid UploadedByUserId { get; set; }
    public User UploadedByUser { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string StoragePath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSizeBytes { get; set; }
    public string Purpose { get; set; } = null!;
}
