using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Files.DTOs;
using MedicHp.Domain.Entities.Core;
using DomainFile = MedicHp.Domain.Entities.Core.File;

namespace MedicHp.Application.Features.Files.Queries.GetPatientFiles;

public class GetPatientFilesQueryHandler : IRequestHandler<GetPatientFilesQuery, List<FileDto>>
{
    private readonly IGenericRepository<DomainFile> _fileRepository;

    public GetPatientFilesQueryHandler(IGenericRepository<DomainFile> fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<List<FileDto>> Handle(GetPatientFilesQuery request, CancellationToken cancellationToken)
    {
        var files = await _fileRepository.GetAsync(
            f => f.UploadedByUserId == request.UserId && (request.Purpose == null || f.Purpose == request.Purpose),
            null,
            cancellationToken);

        return files.Select(f => new FileDto
        {
            Id = f.Id,
            FileName = f.FileName,
            StoragePath = f.StoragePath,
            ContentType = f.ContentType,
            FileSizeBytes = f.FileSizeBytes,
            Purpose = f.Purpose,
            CreatedAt = f.CreatedAt
        }).OrderByDescending(f => f.CreatedAt).ToList();
    }
}
