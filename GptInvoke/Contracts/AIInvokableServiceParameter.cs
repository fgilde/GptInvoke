namespace GptInvoke.Contracts;

public class AIInvokableServiceParameter
{
    public AIInvokableServiceParameter(string name, string description, Type type, bool isRequired)
    {
        Name = name;
        Description = description;
        Type = type;
        IsRequired = isRequired;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public Type Type { get; set; }
    public bool IsRequired { get; set; }
}