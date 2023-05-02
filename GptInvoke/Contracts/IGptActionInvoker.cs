using OneOf;
using OpenAI.Chat;

namespace GptInvoke.Contracts;

public interface IGptActionInvoker
{
    Task ClearHistoryAsync();

    IEnumerable<ChatPrompt> History { get; }

    void SetHistory(IEnumerable<ChatPrompt> history);

    Task<OneOf<string, GptServiceResult>> PromptAsync(string userCommand, Action<string>? resultHandler = null,
        CancellationToken cancellationToken = default);
}