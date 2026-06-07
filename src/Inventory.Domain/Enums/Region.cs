using System.Runtime.Serialization;

namespace Inventory.Domain.Enums;

public enum Region
{
    [EnumMember(Value = "US")]
    US,
    [EnumMember(Value = "Europe")]
    Europe,
    [EnumMember(Value = "Asia")]
    Asia
}