using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace GptInvoke.Helper;

internal static class Utils
{
    internal static bool TryParse<T>(string json, out T res)
    {
        res = default;
        try
        {
            res = JsonConvert.DeserializeObject<T>(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static bool TryParsePartial<T>(string input, out T res)
    {
        res = default;
        var jsonStringPattern = @"(\{(?:[^{}]|(?<o>\{)|(?<-o>\}))*(?(o)(?!))\})|(\[(?:[^\[\]]|(?<o>\[)|(?<-o>\]))*(?(o)(?!))\])";
        var regex = new Regex(jsonStringPattern, RegexOptions.Compiled);

        var matches = regex.Matches(input);

        foreach (Match match in matches)
        {
            var potentialJson = match.Value;

            if (TryParse<T>(potentialJson, out res))
            {
                return true;
            }
        }

        return false;
    }
}