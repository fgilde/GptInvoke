namespace GptInvoke.Contracts;

public interface IAIInvokableService
{
    public string Name { get; }
    public string Description {  get; }
    public AIInvokableServiceParameter[] Parameters { get; }

    public Task<bool> ExecuteAsync(IDictionary<string, object> parameters);
}