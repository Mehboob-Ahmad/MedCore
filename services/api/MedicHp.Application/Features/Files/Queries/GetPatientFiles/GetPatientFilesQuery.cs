using System;
using System.Collections.Generic;
using MediatR;
using MedicHp.Application.Features.Files.DTOs;

namespace MedicHp.Application.Features.Files.Queries.GetPatientFiles;

public class GetPatientFilesQuery : IRequest<List<FileDto>>
{
    public Guid UserId { get; set; }
    public string? Purpose { get; set; }

    public GetPatientFilesQuery(Guid userId, string? purpose = null)
    {
        UserId = userId;
        Purpose = purpose;
    }
}
