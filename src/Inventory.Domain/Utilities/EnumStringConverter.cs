using System.Reflection;
using System.Runtime.Serialization;

namespace Inventory.Domain.Utilities;

public static class EnumStringConverter
{
    public static string ToStringValue<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        var type = typeof(TEnum);
        var name = Enum.GetName(type, value);

        if (name is null)
            return string.Empty;

        var field = type.GetField(name);
        var attr = field?.GetCustomAttribute<EnumMemberAttribute>();

        return attr?.Value ?? name;
    }
    public static bool TryParseEnum<TEnum>(string input, out TEnum result) where TEnum : struct, Enum
    {
        var type = typeof(TEnum);

        // 1. EnumMemberAttribute
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attr = field.GetCustomAttribute<EnumMemberAttribute>();
            if (attr?.Value != null &&
                string.Equals(attr.Value, input, StringComparison.OrdinalIgnoreCase))
            {
                result = (TEnum)field.GetValue(null)!;
                return true;
            }
        }

        // 2. Standardowy parse
        return Enum.TryParse(input, ignoreCase: true, out result);
    }
    public static TEnum ParseEnumOrDefault<TEnum>(string input) where TEnum : struct, Enum
    {
        return TryParseEnum(input, out TEnum result) ? result : default;
    }
    public static TEnum ParseEnumOrThrow<TEnum>(string input) where TEnum : struct, Enum
    {
        if (TryParseEnum(input, out TEnum result))
            return result;

        throw new ArgumentException(
            $"Invalid enum value '{input}' for {typeof(TEnum).Name}");
    }
}