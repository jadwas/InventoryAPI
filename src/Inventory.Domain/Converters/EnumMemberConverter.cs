using Inventory.Domain.Utilities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Inventory.Domain.Converters
{
    public class EnumMemberConverter<T> : ValueConverter<T, string> where T : struct, Enum
    {
        public EnumMemberConverter() : base(
            v => EnumStringConverter.ToStringValue(v),
            v => EnumStringConverter.ParseEnumOrDefault<T>(v))
        { }
    }

}
