# Custom Service Invocation with ChatGPT

This package allows you to easily integrate ChatGPT with your custom service execution, such as invoking webhooks or performing actions within your application.

With this package you can achieve very easially something like this
![SAMPLE](https://raw.githubusercontent.com/fgilde/GptInvoke/main/GptInvoke/screenshots/video.gif)
https://raw.githubusercontent.com/fgilde/GptInvoke/main/GptInvoke/screenshots/video.mkv

[![GitHub](https://img.shields.io/badge/GitHub-Source-blue)](https://github.com/fgilde/GptInvoke)
[![NuGet](https://img.shields.io/badge/NuGet-Package-blue)](https://www.nuget.org/packages/GptInvoke)

If you like this package, please star it on [![GitHub](https://img.shields.io/badge/GitHub-Source-blue)](https://github.com/fgilde/GptInvoke) and share it with your friends
If not, you can give a star anyway and let me know what I can improve to make it better for you.

<hr/>

## Table of Contents
- [Installation](#installation)
- [Service Registration](#service-registration)
- [Service Implementation](#service-implementation)
- [Invocation](#invocation)

## Installation

To install the GptInvoke package, run the following command in the Package Manager Console:

```bash
Install-Package GptInvoke
```

or add the Package reference like this 
```xml
<PackageReference Include="GptInvoke" Version="*" />
```

## Service Registration

To register the necessary services, add the following code in your `Program.cs` file:

```csharp
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => logging.ClearProviders())
    .ConfigureServices(services => services.AddGptActionInvoker("YOUR-API-KEY"))
    .Build();
```

You can also specify settings for the action, like this:

```csharp
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => logging.ClearProviders())
    .ConfigureServices(services => services.AddGptActionInvoker(s =>
    {
        s.ApiKey = "API-KEY";
        s.Model = Model.GPT4;
    }))
    .Build();
```

## Service Implementation

To implement a custom service, create a new class that implements the `IGptInvokableService` interface:

```csharp
public interface IGptInvokableService
{
    public string Name { get; }
    public string Description {  get; }
    public GptInvokableServiceParameter[] Parameters { get; }
    public Task<bool> ExecuteAsync(IDictionary<string, object> parameters);
}
```

Sample implementation:

```csharp
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

Services can then invoked like this 

![Screenshot](https://raw.githubusercontent.com/fgilde/GptInvoke/main/GptInvoke/screenshots/Clock_DE.png)

![Screenshot](https://raw.githubusercontent.com/fgilde/GptInvoke/main/GptInvoke/screenshots/Product_EN.png)


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

The `PromptAsync` method returns a `OneOf<string, GptServiceResult>` object. If the result is a `GptServiceResult`, the service has been invoked and you will receive the following class:

```csharp
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
