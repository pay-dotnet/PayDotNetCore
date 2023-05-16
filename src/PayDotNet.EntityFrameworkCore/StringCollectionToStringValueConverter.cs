using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PayDotNet.EntityFrameworkCore;

internal class StringCollectionToStringValueConverter : ValueConverter<List<string>, string>
{
    private const char Separator = ',';

    public StringCollectionToStringValueConverter()
        : base(v => ToDatabaseValue(v), v => FromDatabaseValue(v))
    {
    }

    public static List<string> FromDatabaseValue(string value)
    {
        return value.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static string ToDatabaseValue(List<string> value)
    {
        return string.Join(Separator, value);
    }
}