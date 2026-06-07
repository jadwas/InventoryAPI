using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}