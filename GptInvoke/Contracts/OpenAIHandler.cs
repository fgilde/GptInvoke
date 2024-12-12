namespace GptInvoke.Contracts;

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

    private Role FindRole(string role)
    {
        // "user", "assistant", "system"
        if (role.Equals("user", StringComparison.InvariantCultureIgnoreCase))
            return Role.User;
        if (role.Equals("assistant", StringComparison.InvariantCultureIgnoreCase))
            return Role.Assistant;
        if (role.Equals("system", StringComparison.InvariantCultureIgnoreCase))
            return Role.System;
        if (role.Equals("tool", StringComparison.InvariantCultureIgnoreCase))
            return Role.Tool;
        return Role.User;
    }

    public async Task<string> GetCompletionAsync(AIRequest request, CancellationToken cancellationToken = default)
    {
        var chatRequest = new ChatRequest(
            messages: request.Messages.Select(m => new Message(FindRole(m.Role), m.Content)).ToList(),
            model: request.Model ?? Model.GPT3_5_Turbo
        );

        var response = await _openAiClient.ChatEndpoint.GetCompletionAsync(chatRequest, cancellationToken);
        return response.FirstChoice.ToString();
    }

    public async Task StreamCompletionAsync(AIRequest request, Action<string> responseHandler, CancellationToken cancellationToken = default)
    {
        var chatRequest = new ChatRequest(
            messages: request.Messages.Select(m => new Message(FindRole(m.Role), m.Content)).ToList(),
            model: request.Model ?? _model ?? Model.GPT3_5_Turbo
        );

        await _openAiClient.ChatEndpoint.StreamCompletionAsync(chatRequest, response =>
        {
            var part = response.FirstChoice.ToString();
            responseHandler(part);
        }, cancellationToken: cancellationToken);
    }
}
