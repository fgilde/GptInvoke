using GptInvoke.Contracts;

namespace GptBotTest.MyServices;

public class MyProductService : IGptInvokableService
{
    public string Name { get; set; } = "Add Product service";
    public string Description { get; set; } = "I can add products";

    public GptInvokableServiceParameter[] Parameters => new[]
    {
        new GptInvokableServiceParameter("Name", "Name of the product to add", typeof(string), true),
        new GptInvokableServiceParameter("Barcode", "Barcode of product", typeof(string), true),
        new GptInvokableServiceParameter("Description", "Description of product", typeof(string), false),
        new GptInvokableServiceParameter("Brand", "Brand for this product", typeof(string), true),
        new GptInvokableServiceParameter("Rate", "Description of product", typeof(decimal), true),
    };

    public Task<bool> ExecuteAsync(IDictionary<string, object> parameters)
    {
        var name = parameters["Name"].ToString();
        var barcode = parameters["Barcode"].ToString();
        ConsoleHelper.WriteLineInColor($"HELLO from {GetType()} I created a new Product with the Name {name} and the Barcode {barcode}", ConsoleColor.Green);
        return Task.FromResult(true);
    }
}