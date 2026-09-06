using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MedicHp.Application.Features.AI.Interfaces;
using MedicHp.Application.Features.AI.DTOs;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MedicHp.Infrastructure.Services.AI;

public class GemmaAIService : IAIAssistant
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GemmaAIService> _logger;

    public GemmaAIService(HttpClient httpClient, ILogger<GemmaAIService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetResponseAsync(string userPrompt, string systemContext = "", IEnumerable<AiMessageDto>? history = null)
    {
        // Getting Gemma key from Environment variables exactly as required.
        var apiKey = Environment.GetEnvironmentVariable("Gemma");

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("Gemma API Key is missing from environment variables.");
            throw new InvalidOperationException("AI Service is temporarily unavailable due to configuration missing.");
        }

        // Create Domain Guard system prompt
        string domainGuardPrompt = @"You are the MedicHp AI Assistant. You act STRICTLY as a 'Domain Guard'.
Constraint 1: You MUST detect if the question is medically related or platform-specific. If it is NOT, you MUST politely refuse to answer.
Constraint 2: You must NOT provide diagnostic or prescriptive medical advice. You must only provide general information and urge the patient to book an appointment with a doctor on the platform.
" + systemContext;

        var contentsList = new System.Collections.Generic.List<object>();
        
        if (history != null)
        {
            foreach(var msg in history)
            {
                contentsList.Add(new {
                    role = msg.Role,
                    parts = new[] { new { text = msg.Content } }
                });
            }
        }
        
        contentsList.Add(new {
            role = "user",
            parts = new[] { new { text = (contentsList.Count == 0 ? domainGuardPrompt + "\n\n" : "") + "User Question: " + userPrompt } }
        });

        var requestBody = new
        {
            contents = contentsList
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        // Example Google Gemini API endpoint for Gemma models, adapt to actual Gemma API endpoint provided by Google AI Studio
        var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemma-2-9b-it:generateContent?key={apiKey}";

        try
        {
            var response = await _httpClient.PostAsync(requestUrl, jsonContent);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemma API returned {StatusCode}: {ErrorBody}", response.StatusCode, errorBody);
                throw new Exception("AI Assistant failed to respond.");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseString);
            
            // Navigate Google API response structure
            var text = document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "I am unable to process this request.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to Gemma API.");
            return "I apologize, but I am currently unavailable. Please try again later.";
        }
    }
}
