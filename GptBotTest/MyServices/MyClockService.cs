using GptInvoke.Contracts;

namespace GptBotTest.MyServices;

public class MyClockService : IGptInvokableService
{
    public string Name { get; set; } = "Alarm clock service";
    public string Description { get; set; } = "I can set up an alarm clock";
    public GptInvokableServiceParameter[] Parameters => new []
    {
        new GptInvokableServiceParameter("DateTime", "Target time to set alarm for", typeof(DateTime), true)
    };

    public Task<bool> ExecuteAsync(IDictionary<string, object> parameters)
    {
        var value = parameters.First().Value;
        var dt = value is DateTime time ? time : DateTime.Parse(value.ToString());
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"HELLO from {GetType()} I created a new alarm at {dt}"); 
        Console.ForegroundColor = color;
        return Task.FromResult(true);
    }
}