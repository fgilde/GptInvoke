using GptInvoke.Contracts;

namespace GptBotTest.MyServices;

public class MyProductService : IAIInvokableService
{
    public string Name { get; set; } = "Add Product service";
    public string Description { get; set; } = "I can add products";

    public AIInvokableServiceParameter[] Parameters => new[]
    {
        new AIInvokableServiceParameter("Name", "Name of the product to add", typeof(string), true),
        new AIInvokableServiceParameter("Barcode", "Barcode of product", typeof(string), true),
        new AIInvokableServiceParameter("Description", "Description of product", typeof(string), false),
        new AIInvokableServiceParameter("Brand", "Brand for this product", typeof(string), true),
        new AIInvokableServiceParameter("Rate", "Description of product", typeof(decimal), true),
    };

    public Task<bool> ExecuteAsync(IDictionary<string, object> parameters)
    {
        var name = parameters["Name"].ToString();
        var barcode = parameters["Barcode"].ToString();
        ConsoleHelper.WriteLineInColor($"HELLO from {GetType()} I created a new Product with the Name {name} and the Barcode {barcode}", ConsoleColor.Green);
        return Task.FromResult(true);
    }
}