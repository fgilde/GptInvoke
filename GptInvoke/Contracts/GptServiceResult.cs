namespace GptInvoke.Contracts;

public class GptServiceResult
{
    public string Service { get; set; }
    public string Type { get; set; }
    public KeyValuePair<string, object>[] Parameters { get; set; }
    public bool Successful { get; set; }
    public IGptInvokableService UsedService { get; set; }
}
