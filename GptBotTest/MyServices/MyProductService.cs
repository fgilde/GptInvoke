using GptInvoke.Contracts;

namespace GptBotTest.MyServices;

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