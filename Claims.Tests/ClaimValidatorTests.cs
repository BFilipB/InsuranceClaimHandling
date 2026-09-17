using Claims.Models;
using Claims.Validation;
using Xunit;

namespace Claims.Tests
{
    public class ClaimValidatorTests
    {
        private readonly ClaimValidator _validator = new();

        private static Cover MakeCover(DateTime start, DateTime end) => new()
        {
            Id = "cover-1",
            StartDate = start,
            EndDate = end,
            Type = CoverType.Yacht
        };

        [Fact]
        public void DamageCostOverLimit_Throws()
        {
            var cover = MakeCover(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1));
            var claim = new Claim { CoverId = cover.Id, DamageCost = 100_001m, Created = new DateTime(2026, 2, 1) };

            Assert.Throws<ValidationException>(() => _validator.Validate(claim, cover));
        }

        [Fact]
        public void DamageCostAtLimit_DoesNotThrow()
        {
            var cover = MakeCover(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1));
            var claim = new Claim { CoverId = cover.Id, DamageCost = 100_000m, Created = new DateTime(2026, 2, 1) };

            var exception = Record.Exception(() => _validator.Validate(claim, cover));

            Assert.Null(exception);
        }

        [Fact]
        public void CreatedDateBeforeCoverStart_Throws()
        {
            var cover = MakeCover(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1));
            var claim = new Claim { CoverId = cover.Id, DamageCost = 100m, Created = new DateTime(2025, 12, 31) };

            Assert.Throws<ValidationException>(() => _validator.Validate(claim, cover));
        }

        [Fact]
        public void CreatedDateAfterCoverEnd_Throws()
        {
            var cover = MakeCover(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1));
            var claim = new Claim { CoverId = cover.Id, DamageCost = 100m, Created = new DateTime(2026, 7, 1) };

            Assert.Throws<ValidationException>(() => _validator.Validate(claim, cover));
        }

        [Fact]
        public void CreatedDateWithinCoverPeriod_DoesNotThrow()
        {
            var cover = MakeCover(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1));
            var claim = new Claim { CoverId = cover.Id, DamageCost = 100m, Created = new DateTime(2026, 3, 1) };

            var exception = Record.Exception(() => _validator.Validate(claim, cover));

            Assert.Null(exception);
        }

        [Fact]
        public void MissingRelatedCover_Throws()
        {
            var claim = new Claim { CoverId = "does-not-exist", DamageCost = 100m, Created = DateTime.UtcNow };

            Assert.Throws<ValidationException>(() => _validator.Validate(claim, null));
        }
    }
}
