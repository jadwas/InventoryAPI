using Inventory.Domain.Enums;
using Inventory.Infrastructure.Services;

namespace Inventory.Tests.Unit.Domain.Common.Interfaces
{
    public class PricingServiceTests
    {
        private readonly PricingService _service = new PricingService();

        [Theory]
        [InlineData(Region.US, 1.00)]
        [InlineData(Region.Europe, 1.15)]
        [InlineData(Region.Asia, 1.05)]
        public void RegionBasedMultiplier_ShouldReturnCorrectMultiplier(Region region, decimal expected)
        {
            // Act
            var result = _service.RegionBasedMultiplier(region);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RegionBasedMultiplier_InvalidRegion_ShouldThrow()
        {
            // Arrange
            var invalid = (Region)999;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _service.RegionBasedMultiplier(invalid));
        }
    }
}