﻿namespace GptInvoke.Contracts;

using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

public class OpenAiHandler : IAIHandler
{
    private readonly Model? _model;
    private readonly OpenAIClient _openAiClient;

    public OpenAiHandler(string apiKey, Model? model = null)
    {
        _model = model;
        _openAiClient = new OpenAIClient(apiKey);
    }

    public async Task<string> GetCompletionAsync(AIRequest request, CancellationToken cancellationToken = default)
    {
        var chatRequest = new ChatRequest(
            messages: request.Messages.Select(m => new ChatPrompt(m.Role, m.Content)).ToList(),
            model: request.Model ?? Model.GPT3_5_Turbo
        );

        var response = await _openAiClient.ChatEndpoint.GetCompletionAsync(chatRequest, cancellationToken);
        return response.FirstChoice.ToString();
    }

    public async Task StreamCompletionAsync(AIRequest request, Action<string> responseHandler, CancellationToken cancellationToken = default)
    {
        var chatRequest = new ChatRequest(
            messages: request.Messages.Select(m => new ChatPrompt(m.Role, m.Content)).ToList(),
            model: request.Model ?? _model ?? Model.GPT3_5_Turbo
        );

        await _openAiClient.ChatEndpoint.StreamCompletionAsync(chatRequest, response =>
        {
            var part = response.FirstChoice.ToString();
            responseHandler(part);
        }, cancellationToken);
    }
}