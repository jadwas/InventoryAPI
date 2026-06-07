using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public Region Region { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}