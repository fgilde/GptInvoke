using GptInvoke;
using GptInvoke.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello spooky world where the AI will soon rule!");

var apiKey = "<YOUR API KEY>";


using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => logging.ClearProviders())
    .ConfigureServices(services => services.AddGptActionInvoker(apiKey))
    .Build();


_= host.RunAsync();
var commander = host.Services.GetRequiredService<IGptActionInvoker>();
Console.WriteLine("How can I assist you?");
while (true)
{
    var userCommand = Console.ReadLine();
    if (!string.IsNullOrEmpty(userCommand))
    {
        var res = await commander.PromptAsync(userCommand);
        res.Switch(Console.WriteLine, _ => Console.WriteLine("####################################" + Environment.NewLine));
    }
}


