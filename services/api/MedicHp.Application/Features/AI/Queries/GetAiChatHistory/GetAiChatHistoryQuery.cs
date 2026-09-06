using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MedicHp.Application.Common;
using MedicHp.Application.Features.AI.DTOs;
using MedicHp.Domain.Entities.Clinical;

namespace MedicHp.Application.Features.AI.Queries.GetAiChatHistory;

public class GetAiChatHistoryQuery : IRequest<List<AiMessageDto>>
{
    public Guid UserId { get; set; }
}

public class GetAiChatHistoryQueryHandler : IRequestHandler<GetAiChatHistoryQuery, List<AiMessageDto>>
{
    private readonly IGenericRepository<AiChatMessage> _aiMessageRepository;

    public GetAiChatHistoryQueryHandler(IGenericRepository<AiChatMessage> aiMessageRepository)
    {
        _aiMessageRepository = aiMessageRepository;
    }

    public async Task<List<AiMessageDto>> Handle(GetAiChatHistoryQuery request, CancellationToken cancellationToken)
    {
        var history = await _aiMessageRepository.GetAsync(
            m => m.UserId == request.UserId,
            include: null,
            cancellationToken: cancellationToken);
            
        return history.OrderBy(m => m.CreatedAt).Select(m => new AiMessageDto
        {
            Role = m.Role,
            Content = m.Content
        }).ToList();
    }
}
