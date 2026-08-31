using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Files.DTOs;
using MedicHp.Domain.Entities.Core;

namespace MedicHp.Application.Features.Files.Queries.GetPatientFiles;

public class GetPatientFilesQueryHandler : IRequestHandler<GetPatientFilesQuery, List<FileDto>>
{
    private readonly IGenericRepository<File> _fileRepository;

    public GetPatientFilesQueryHandler(IGenericRepository<File> fileRepository)
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
