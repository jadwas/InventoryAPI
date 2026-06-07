using Inventory.Domain.Entities;
    using Inventory.Infrastructure.Services;
    using System.Reflection;
    using Inventory.Domain.Enums;

    namespace Inventory.Tests.Integration.Domain
{
  

    public class PricingEndToEndTests
    {
        private readonly PricingService _pricing = new PricingService();
        private readonly DiscountPolicy _discount = new DiscountPolicy();

        public PricingEndToEndTests()
        {
            typeof(DiscountPolicy)
                .GetField("_bankHolidays", BindingFlags.NonPublic | BindingFlags.Static)
                ?.SetValue(null, null);
        }

        private Product P(decimal price) => new Product
        {
            Id = Guid.NewGuid(),
            Name = "Laptop",
            Price = price
        };

        [Fact]
        public void EndToEnd_BlackFriday_Europe_10Units()
        {
            var product = P(2000m);
            var date = new DateOnly(2026, 11, 27).ToDateTime(TimeOnly.MinValue);

            var discounted = _discount.CalculateDiscount(1, product, product.Price, 10, date);
            var multiplier = _pricing.RegionBasedMultiplier(Region.Europe);

            var finalPrice = discounted * multiplier;

            // 2000 → 25% off = 1600
            // Europe +15% = 1725
            Assert.Equal(1725m, finalPrice);
        }

        [Fact]
        public void EndToEnd_Holiday_US_FirstPosition()
        {
            var product = P(500m);
            var date = new DateTime(2026, 1, 1); // holiday

            var discounted = _discount.CalculateDiscount(0, product, product.Price, 1, date);
            var multiplier = _pricing.RegionBasedMultiplier(Region.US);

            var finalPrice = discounted * multiplier;

            // 500 → 15% off = 425
            Assert.Equal(425m, finalPrice);
        }

        [Fact]
        public void EndToEnd_NormalDay_Asia_100Units()
        {
            var product = P(50m);
            var date = new DateTime(2026, 3, 10);

            var discounted = _discount.CalculateDiscount(1, product, product.Price, 100, date);
            var multiplier = _pricing.RegionBasedMultiplier(Region.Asia);

            var finalPrice = discounted * multiplier;

            // 50 → 30% off = 35
            // Asia +5% = 36.75
            Assert.Equal(36.75m, finalPrice);
        }
    }

}
