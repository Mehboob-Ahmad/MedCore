using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MedicHp.Application.Common;
using MedicHp.Application.Features.AI.DTOs;
using MedicHp.Application.Features.AI.Interfaces;
using MedicHp.Domain.Entities.Clinical;

namespace MedicHp.Application.Features.AI.Commands.AskAi;

public class AskAiCommand : IRequest<string>
{
    public Guid UserId { get; set; }
    public string Prompt { get; set; } = string.Empty;
}

public class AskAiCommandHandler : IRequestHandler<AskAiCommand, string>
{
    private readonly IAIAssistant _aiAssistant;
    private readonly IGenericRepository<AiChatMessage> _aiMessageRepository;

    public AskAiCommandHandler(
        IAIAssistant aiAssistant, 
        IGenericRepository<AiChatMessage> aiMessageRepository)
    {
        _aiAssistant = aiAssistant;
        _aiMessageRepository = aiMessageRepository;
    }

    public async Task<string> Handle(AskAiCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch previous history for context
        var history = await _aiMessageRepository.GetAsync(
            m => m.UserId == request.UserId,
            include: null,
            cancellationToken: cancellationToken);
            
        var dtos = history.OrderBy(m => m.CreatedAt).Select(m => new AiMessageDto
        {
            Role = m.Role,
            Content = m.Content
        }).ToList();

        // 2. Save user's new message
        var userMessage = new AiChatMessage
        {
            UserId = request.UserId,
            Role = "user",
            Content = request.Prompt
        };
        await _aiMessageRepository.AddAsync(userMessage, cancellationToken);

        // 3. Ask AI
        var systemContext = "The user is a patient asking a medical or platform-related question.";
        var responseText = await _aiAssistant.GetResponseAsync(request.Prompt, systemContext, dtos);

        // 4. Save AI's response
        var aiMessage = new AiChatMessage
        {
            UserId = request.UserId,
            Role = "model",
            Content = responseText
        };
        await _aiMessageRepository.AddAsync(aiMessage, cancellationToken);

        return responseText;
    }
}
