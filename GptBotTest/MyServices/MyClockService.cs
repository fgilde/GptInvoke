using GptInvoke.Contracts;

namespace GptBotTest.MyServices;

public class MyClockService : IAIInvokableService
{
    public string Name { get; set; } = "Alarm clock service";
    public string Description { get; set; } = "I can set up an alarm clock";
    public AIInvokableServiceParameter[] Parameters => new []
    {
        new AIInvokableServiceParameter("DateTime", "Target time to set alarm for", typeof(DateTime), true)
    };

    public Task<bool> ExecuteAsync(IDictionary<string, object> parameters)
    {
        var value = parameters.First().Value;
        var dt = value is DateTime time ? time : DateTime.Parse(value.ToString());
        ConsoleHelper.WriteLineInColor($"HELLO from {GetType()} I created a new alarm at {dt}", ConsoleColor.Green); 
        return Task.FromResult(true);
    }
}