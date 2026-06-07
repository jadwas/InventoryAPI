using Inventory.Domain.Entities;

namespace Inventory.Application.Common.Interfaces;

public interface IDiscountPolicy
{
    decimal CalculateDiscount(int position, Product product, decimal unitPrice, int quantity, DateTime date);
}