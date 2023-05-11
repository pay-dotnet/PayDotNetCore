namespace PayDotNet.Core;

public static class Helpers
{
    public static bool None<T>(this IEnumerable<T> values) => !values.Any();

    public static string? Try(this Dictionary<string, string> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }
        return null;
    }

    public static string TryOrDefault(this Dictionary<string, string> dictionary, string key, string defaultValue)
    {
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }
        return defaultValue;
    }
}