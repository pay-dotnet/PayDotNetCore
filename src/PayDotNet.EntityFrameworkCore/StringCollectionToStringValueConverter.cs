using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PayDotNet.EntityFrameworkCore;

internal class StringCollectionToStringValueConverter : ValueConverter<ICollection<string>, string>
{
    private const char Separator = ',';

    public StringCollectionToStringValueConverter()
        : base(v => ToDatabaseValue(v), v => FromDatabaseValue(v))
    {
    }

    public static ICollection<string> FromDatabaseValue(string value)
    {
        return value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string ToDatabaseValue(ICollection<string> value)
    {
        return string.Join(Separator, value);
    }
}