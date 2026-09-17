using Claims.Models;
using Claims.Services;
using Xunit;

namespace Claims.Tests
{
    public class PremiumCalculatorTests
    {
        private readonly PremiumCalculator _calculator = new();

        [Theory]
        [InlineData(CoverType.Yacht, 1.10)]
        [InlineData(CoverType.PassengerShip, 1.20)]
        [InlineData(CoverType.Tanker, 1.50)]
        [InlineData(CoverType.ContainerShip, 1.30)]
        [InlineData(CoverType.BulkCarrier, 1.30)]
        public void ExactlyThirtyDays_UsesFullRateOnly_NoDiscount(CoverType type, double multiplier)
        {
            var start = new DateTime(2026, 1, 1);
            var end = start.AddDays(30);

            var result = _calculator.Compute(start, end, type);

            var expected = 30 * 1250m * (decimal)multiplier;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ExactlyOneHundredEightyDays_Yacht_AppliesTier2DiscountOnly()
        {
            var start = new DateTime(2026, 1, 1);
            var end = start.AddDays(180);

            var result = _calculator.Compute(start, end, CoverType.Yacht);

            var premiumPerDay = 1250m * 1.10m;
            var expected = 30 * premiumPerDay + 150 * premiumPerDay * 0.95m;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ExactlyOneHundredEightyDays_NonYacht_AppliesTier2DiscountOnly()
        {
            var start = new DateTime(2026, 1, 1);
            var end = start.AddDays(180);

            var result = _calculator.Compute(start, end, CoverType.ContainerShip);

            var premiumPerDay = 1250m * 1.30m;
            var expected = 30 * premiumPerDay + 150 * premiumPerDay * 0.98m;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MoreThanOneHundredEightyDays_Yacht_AppliesCumulativeTier3Discount()
        {
            var start = new DateTime(2026, 1, 1);
            var end = start.AddDays(200);

            var result = _calculator.Compute(start, end, CoverType.Yacht);

            var premiumPerDay = 1250m * 1.10m;
            var expected = 30 * premiumPerDay
                         + 150 * premiumPerDay * 0.95m
                         + 20 * premiumPerDay * 0.92m; // 8% total off base for the remaining 20 days
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MoreThanOneHundredEightyDays_NonYacht_AppliesCumulativeTier3Discount()
        {
            var start = new DateTime(2026, 1, 1);
            var end = start.AddDays(200);

            var result = _calculator.Compute(start, end, CoverType.Tanker);

            var premiumPerDay = 1250m * 1.50m;
            var expected = 30 * premiumPerDay
                         + 150 * premiumPerDay * 0.98m
                         + 20 * premiumPerDay * 0.97m; // 3% total off base for the remaining 20 days
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FewerThanThirtyDays_OnlyChargesForThoseDaysAtFullRate()
        {
            var start = new DateTime(2026, 1, 1);
            var end = start.AddDays(10);

            var result = _calculator.Compute(start, end, CoverType.Yacht);

            var expected = 10 * 1250m * 1.10m;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ZeroLengthCover_ReturnsZero()
        {
            var start = new DateTime(2026, 1, 1);

            var result = _calculator.Compute(start, start, CoverType.Yacht);

            Assert.Equal(0m, result);
        }

        [Fact]
        public void NegativeLengthCover_ReturnsZero()
        {
            var start = new DateTime(2026, 1, 10);
            var end = new DateTime(2026, 1, 1);

            var result = _calculator.Compute(start, end, CoverType.Yacht);

            Assert.Equal(0m, result);
        }
    }
}
