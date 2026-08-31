using System;
using System.IO;
using System.Threading.Tasks;
using MedicHp.Application.Features.Files.Commands.CreateFileMetadata;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicHp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/files")]
public class FilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("upload")]
    [AllowAnonymous] // Allow anonymous for registration documents
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string purpose = "General")
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { success = false, message = "No file uploaded." });

        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
        if (!System.Linq.Enumerable.Contains(allowedContentTypes, file.ContentType.ToLower()))
        {
            return BadRequest(new { success = false, message = "Only PDF and image files are allowed." });
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var command = new CreateFileMetadataCommand
        {
            FileName = file.FileName,
            StoragePath = $"/uploads/{uniqueFileName}",
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            Purpose = purpose
        };

        var fileId = await _mediator.Send(command);

        return Ok(new { success = true, fileId = fileId, url = command.StoragePath });
    }
}
