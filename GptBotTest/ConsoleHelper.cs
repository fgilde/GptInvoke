namespace GptBotTest;

internal partial class ConsoleHelper
{
    public static void WriteLineInColor(string? s, ConsoleColor color)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(s);
        Console.ForegroundColor = oldColor;
    }
}