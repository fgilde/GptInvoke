using GptBotTest;
using GptInvoke;
using GptInvoke.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenAI.Models;

bool liveOutput = true;
Console.WriteLine("Hello spooky world where the AI will soon rule!");
AppDomain.CurrentDomain.UnhandledException += (_, args) => ConsoleHelper.WriteLineInColor(args.ExceptionObject.ToString(), ConsoleColor.DarkRed);
AppDomain.CurrentDomain.ProcessExit += (_, _) => Console.WriteLine("Goodbye cruel world!");

var apiKey = Environment.GetEnvironmentVariable("GPT_API_KEY", EnvironmentVariableTarget.User) ?? "<YOUR API KEY>";

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => logging.ClearProviders())
    .ConfigureServices(services => services.AddOpenAIActionInvoker(settings =>
    {
        settings.ApiKey = apiKey;
        settings.Model = Model.GPT4; // YOU should use GPT-4 if you can
    }))
    .Build();


_= host.RunAsync();
var commander = host.Services.GetRequiredService<IAIActionInvoker>();
Console.WriteLine("How can I assist you?");
while (true)
{
    Console.WriteLine();
    Console.Write("User: ");
    var userCommand = Console.ReadLine();
    if (userCommand == "clear")
        await commander.ClearHistoryAsync();
    else if (!string.IsNullOrEmpty(userCommand))
    {
        Console.Write("AI: ");
        var res = await commander.PromptAsync(userCommand, liveOutput ? Console.Write : null);
        if (!liveOutput)
            res.Switch(Console.WriteLine, _ => Console.WriteLine("####################################" + Environment.NewLine));
        else
            res.Switch(_ => { }, _ => Console.WriteLine("####################################" + Environment.NewLine));
    }
}


