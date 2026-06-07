using System.Runtime.Serialization;

namespace Inventory.Domain.Enums;

public enum OrderStatus
{
    [EnumMember(Value = "new")]
    New,

    [EnumMember(Value = "processing")]
    Processing,

    [EnumMember(Value = "completed")]
    Completed,

    [EnumMember(Value = "cancelled")]
    Cancelled
}
