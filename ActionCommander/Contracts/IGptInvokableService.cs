namespace GptInvoke.Contracts;

public interface IGptInvokableService
{
    public string Name { get; }
    public string Description {  get; }
    public GptInvokableServiceParameter[] Parameters { get; }

    public Task<bool> ExecuteAsync(IDictionary<string, object> parameters);
}