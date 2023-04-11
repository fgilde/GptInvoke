using OneOf;

namespace GptInvoke.Contracts;

public interface IGptActionInvoker
{
    Task ClearHistoryAsync();

    Task<OneOf<string, GptServiceResult>> PromptAsync(string userCommand, Action<string>? resultHandler = null,
        CancellationToken cancellationToken = default);
}