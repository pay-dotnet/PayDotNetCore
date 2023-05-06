namespace PayDotNet.Core;

public static class Helpers
{
    public static bool None<T>(this IEnumerable<T> values) => !values.Any();
}