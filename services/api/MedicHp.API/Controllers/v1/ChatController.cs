using System;
using System.Threading.Tasks;
using MedicHp.Application.Features.Auth.Interfaces;
using MedicHp.Application.Features.Chat.Commands.SendMessage;
using MedicHp.Application.Features.Chat.Queries.GetConversations;
using MedicHp.Application.Features.Chat.Queries.GetMessages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicHp.Api.Controllers.v1;

[ApiController]
[Route("api/v1/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ChatController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetConversationsQuery(userId.Value);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpGet("conversations/{id}/messages")]
    public async Task<IActionResult> GetMessages(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var query = new GetMessagesQuery(userId.Value, id);
        var result = await _mediator.Send(query);

        return Ok(new { success = true, data = result });
    }

    [HttpPost("conversations/{id}/messages")]
    public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        command.ConversationId = id;
        command.UserId = userId.Value;
        
        var result = await _mediator.Send(command);

        return Created("", new { success = true, data = result });
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> CreateOrGetConversation([FromBody] MedicHp.Application.Features.Chat.Commands.CreateOrGetConversation.CreateOrGetConversationCommand command)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        command.UserId = userId.Value;
        
        var result = await _mediator.Send(command);
        return Ok(new { success = true, data = result });
    }

    [HttpPost("conversations/{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var command = new MedicHp.Application.Features.Chat.Commands.MarkConversationRead.MarkConversationReadCommand
        {
            ConversationId = id,
            UserId = userId.Value
        };
        
        await _mediator.Send(command);
        return Ok(new { success = true });
    }

    [HttpPost("attachments")]
    public async Task<IActionResult> UploadAttachment(Microsoft.AspNetCore.Http.IFormFile file)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        if (file == null || file.Length == 0)
            return BadRequest(new { success = false, message = "No file uploaded." });

        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "video/mp4", "audio/mpeg", "audio/ogg", "audio/webm" };
        if (!System.Linq.Enumerable.Contains(allowedContentTypes, file.ContentType.ToLower()))
        {
            return BadRequest(new { success = false, message = "Invalid file type." });
        }

        var storageFolder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "storage", "chat-media");
        if (!System.IO.Directory.Exists(storageFolder))
            System.IO.Directory.CreateDirectory(storageFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = System.IO.Path.Combine(storageFolder, uniqueFileName);

        using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var command = new MedicHp.Application.Features.Files.Commands.CreateFileMetadata.CreateFileMetadataCommand
        {
            FileName = file.FileName,
            StoragePath = filePath,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            Purpose = "Chat"
        };

        var fileId = await _mediator.Send(command);

        return Ok(new { success = true, data = new { attachmentId = fileId, url = $"/api/v1/chat/attachments/{fileId}" } });
    }

    [HttpGet("attachments/{id}")]
    public async Task<IActionResult> GetAttachment(Guid id, [FromServices] MedicHp.Application.Common.IGenericRepository<MedicHp.Domain.Entities.Core.File> fileRepository, [FromServices] MedicHp.Application.Common.IGenericRepository<MedicHp.Domain.Entities.Messaging.ChatMessage> messageRepository)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Unauthorized();

        // Security Check: Does this user have access to a conversation containing this attachment?
        var message = await messageRepository.FirstOrDefaultAsync(
            m => m.AttachmentId == id && (m.Conversation.PatientId == userId.Value || m.Conversation.DoctorId == userId.Value),
            null);
            
        if (message == null) return Forbid();

        var fileInfo = await fileRepository.GetByIdAsync(id);
        if (fileInfo == null || !System.IO.File.Exists(fileInfo.StoragePath))
            return NotFound();

        var stream = new System.IO.FileStream(fileInfo.StoragePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
        return File(stream, fileInfo.ContentType, enableRangeProcessing: true);
    }
}
