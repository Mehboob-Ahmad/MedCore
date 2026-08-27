using System;
using System.Threading;
using System.Threading.Tasks;
using MedicHp.Application.Common;
using MedicHp.Application.Features.Auth.Interfaces;
using DomainFile = MedicHp.Domain.Entities.Core.File;
using MediatR;

namespace MedicHp.Application.Features.Files.Commands.CreateFileMetadata;

public class CreateFileMetadataCommand : IRequest<Guid>
{
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string Purpose { get; set; } = string.Empty;
}

public class CreateFileMetadataCommandHandler : IRequestHandler<CreateFileMetadataCommand, Guid>
{
    private readonly IGenericRepository<DomainFile> _fileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateFileMetadataCommandHandler(
        IGenericRepository<DomainFile> fileRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _fileRepository = fileRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateFileMetadataCommand request, CancellationToken cancellationToken)
    {
        var file = new DomainFile
        {
            FileName = request.FileName,
            StoragePath = request.StoragePath,
            ContentType = request.ContentType,
            FileSizeBytes = request.FileSizeBytes,
            Purpose = request.Purpose,
            // During registration, the user might not be logged in yet.
            UploadedByUserId = _currentUserService.UserId ?? Guid.Empty
        };

        await _fileRepository.AddAsync(file);
        await _unitOfWork.SaveChangesAsync();

        return file.Id;
    }
}
