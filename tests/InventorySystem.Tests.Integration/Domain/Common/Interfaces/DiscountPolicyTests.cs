using System.Reflection;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Services;

namespace Inventory.Tests.Unit.Domain.Common.Interfaces
{
    public class DiscountPolicyTests
    {
        private readonly DiscountPolicy _policy = new DiscountPolicy();

        public DiscountPolicyTests()
        {
            // Reset statycznego cache świąt (_bankHolidays)
            typeof(DiscountPolicy)
                .GetField("_bankHolidays", BindingFlags.NonPublic | BindingFlags.Static)
                ?.SetValue(null, null);
        }

        private Product DummyProduct() => new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Price = 100
        };

        // -----------------------------
        // 1. BLACK FRIDAY
        // -----------------------------
        [Fact]
        public void BlackFriday_ShouldApply25PercentDiscount()
        {
            // Arrange
            var product = DummyProduct();
            var blackFriday = new DateOnly(2026, 11, 27); // 4th Friday of November 2026
            var date = blackFriday.ToDateTime(TimeOnly.MinValue);

            // Act
            var result = _policy.CalculateDiscount(0, product, 100m, 1, date);

            // Assert
            Assert.Equal(75m, result); // 25% off
        }

        // -----------------------------
        // 2. Bank Holiday  & first position
        // -----------------------------
        [Fact]
        public void FirstPosition_OnBankHoliday_ShouldApply15PercentDiscount()
        {
            // Arrange
            var product = DummyProduct();
            var date = new DateTime(2026, 1, 1); // Nowy Rok (święto)

            // Act
            var result = _policy.CalculateDiscount(0, product, 100m, 1, date);

            // Assert
            Assert.Equal(85m, result); // 15% off
        }

        // -----------------------------
        // 3. Quantitive discounts
        // -----------------------------
        [Theory]
        [InlineData(5, 90)] // 10% off
        [InlineData(9, 90)] // 10% off
        [InlineData(10, 80)] // 20% off
        [InlineData(49, 80)] // 20% off
        [InlineData(50, 70)] // 30% off
        [InlineData(100, 70)] // 30% off
        public void QuantityDiscounts_ShouldApplyCorrectDiscount(int quantity, decimal expectedPrice)
        {
            // Arrange
            var product = DummyProduct();
            var date = new DateTime(2026, 3, 10); // non-holiday day

            // Act
            var result = _policy.CalculateDiscount(1, product, 100m, quantity, date);

            // Assert
            Assert.Equal(expectedPrice, result);
        }

        // -----------------------------
        // 4. Biggest discount wins
        // -----------------------------
        [Fact]
        public void MultipleDiscounts_ShouldPickHighest()
        {
            // Arrange
            var product = DummyProduct();

            // 50 pieces → 30%
            // Black Friday → 25%
            // First position + Bank holiday → 15%
            // Expected:
            // Największy = 30%

            var blackFriday = new DateOnly(2026, 11, 27);
            var date = blackFriday.ToDateTime(TimeOnly.MinValue);

            // Act
            var result = _policy.CalculateDiscount(0, product, 100m, 50, date);

            // Assert
            Assert.Equal(70m, result); // 30% off
        }

        // -----------------------------
        // 5. No discounts
        // -----------------------------
        [Fact]
        public void NoDiscounts_ShouldReturnOriginalPrice()
        {
            // Arrange
            var product = DummyProduct();
            var date = new DateTime(2026, 3, 10); // zwykły dzień

            // Act
            var result = _policy.CalculateDiscount(1, product, 100m, 1, date);

            // Assert
            Assert.Equal(100m, result);
        }
    }
}