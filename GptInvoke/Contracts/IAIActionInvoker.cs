using OneOf;
using OpenAI.Chat;

namespace GptInvoke.Contracts;

public interface IAIActionInvoker
{
    Task ClearHistoryAsync();

    IEnumerable<AIMessage> History { get; }

    void SetHistory(IEnumerable<AIMessage> history);

    Task<OneOf<string, AIInvokeServiceResult>> PromptAsync(string userCommand, Action<string>? resultHandler = null,
        CancellationToken cancellationToken = default);
}