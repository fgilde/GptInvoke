namespace GptInvoke.Contracts;

public interface IAIHandler
{
    /// <summary>
    /// Sendet eine Liste von Nachrichten an das LLM und erhält eine einzelne vollständige Antwort zurück.
    /// </summary>
    Task<string> GetCompletionAsync(AIRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sendet eine Liste von Nachrichten an das LLM und streamt die Antwort stückweise.
    /// </summary>
    Task StreamCompletionAsync(AIRequest request, Action<string> responseHandler, CancellationToken cancellationToken = default);
}

public class AIMessage
{
    public string Role { get; set; } // "user", "assistant", "system"
    public string Content { get; set; }
}

public class AIRequest
{
    public string Model { get; set; }
    public List<AIMessage> Messages { get; set; } = new();
}
