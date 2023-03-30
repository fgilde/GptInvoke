# GptInvoke

GptInvoke is a NuGet package that simplifies the integration of ChatGPT with your own service invocation. This allows you to easily combine AI with custom service execution, such as calling a webhook or performing an action in your application.

## Links

- [NuGet Package](https://www.nuget.org/packages/GptInvoke)
- [GitHub Source](https://github.com/fgilde/GptInvoke)

## Installation

To install the GptInvoke package, run the following command in the Package Manager Console:

```bash
Install-Package GptInvoke
```


Service Registration
To register the necessary services, add the following code in your Program.cs file:

```c#
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => logging.ClearProviders())
    .ConfigureServices(services => services.AddGptActionInvoker("YOUR-API-KEY"))
    .Build();

```

```c#
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => logging.ClearProviders())
    .ConfigureServices(services => services.AddGptActionInvoker("YOUR-API-KEY"))
    .Build();

```

You can also specify settings for the action, like this:

```c#
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => logging.ClearProviders())
    .ConfigureServices(services => services.AddGptActionInvoker(s =>
    {
        s.ApiKey = "API-KEY";
        s.Model = Model.GPT4;
    }))
    .Build();


```
Service Implementation
To implement a custom service, create a new class that implements the IGptInvokableService interface:

```c#
public interface IGptInvokableService
{
    public string Name { get; }
    public string Description {  get; }
    public GptInvokableServiceParameter[] Parameters { get; }
    public Task<bool> ExecuteAsync(IDictionary<string, object> parameters);
}
```

Sample implementation
```c#
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


public class MyProductService : IGptInvokableService
{
    public string Name { get; set; } = "Add Product service";
    public string Description { get; set; } = "I can add products";
    public GptInvokableServiceParameter[] Parameters => new[]
    {
        new GptInvokableServiceParameter("Name", "Name of the product to add", typeof(string), true),
        new GptInvokableServiceParameter("Barcode", "Barcode of product", typeof(string), true)
    };

    public Task<bool> ExecuteAsync(IDictionary<string, object> parameters)
    {
        var name = parameters["Name"].ToString();
        var barcode = parameters["Barcode"].ToString();

        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"HELLO from {GetType()} I created a new Product with the Name {name} and the Barcode {barcode}");
        Console.ForegroundColor = color;

        return Task.FromResult(true);
    }
}
```


Using

```c#

## Invocation

To use the `IGptActionInvoker`, add the following code to your application:

```csharp
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

```

The PromptAsync method returns a OneOf<string, GptServiceResult> object. If the result is a GptServiceResult, the service has been invoked and you will receive the following class:

```c#
public class GptServiceResult
{
    public string Service { get; set; }
    public string Type { get; set; }
    public KeyValuePair<string, object>[] Parameters { get; set; }
    public bool Successful { get; set; }
    public IGptInvokableService UsedService { get; set; }
}

```

Now you have all the necessary information to start using GptInvoke in your projects. Simply follow the instructions for installation, service registration, service implementation, and invocation to integrate ChatGPT with your custom service execution.
