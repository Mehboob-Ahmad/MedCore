using System.Collections.Generic;
using System.Threading.Tasks;
using MedicHp.Application.Features.AI.DTOs;

namespace MedicHp.Application.Features.AI.Interfaces;

public interface IAIAssistant
{
    Task<string> GetResponseAsync(string userPrompt, string systemContext = "", IEnumerable<AiMessageDto>? history = null);
}
