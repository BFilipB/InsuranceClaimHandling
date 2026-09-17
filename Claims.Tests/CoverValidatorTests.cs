using Claims.Models;
using Claims.Validation;
using Xunit;

namespace Claims.Tests
{
    public class CoverValidatorTests
    {
        private readonly CoverValidator _validator = new();

        [Fact]
        public void StartDateInPast_Throws()
        {
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.Date.AddDays(-1),
                EndDate = DateTime.UtcNow.Date.AddMonths(1),
                Type = CoverType.Yacht
            };

            Assert.Throws<ValidationException>(() => _validator.Validate(cover));
        }

        [Fact]
        public void StartDateToday_DoesNotThrow()
        {
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddMonths(1),
                Type = CoverType.Yacht
            };

            var exception = Record.Exception(() => _validator.Validate(cover));

            Assert.Null(exception);
        }

        [Fact]
        public void PeriodOverOneYear_Throws()
        {
            var start = DateTime.UtcNow.Date.AddDays(1);
            var cover = new Cover
            {
                StartDate = start,
                EndDate = start.AddDays(366),
                Type = CoverType.Yacht
            };

            Assert.Throws<ValidationException>(() => _validator.Validate(cover));
        }

        [Fact]
        public void PeriodExactlyOneYear_DoesNotThrow()
        {
            var start = DateTime.UtcNow.Date.AddDays(1);
            var cover = new Cover
            {
                StartDate = start,
                EndDate = start.AddDays(365),
                Type = CoverType.Yacht
            };

            var exception = Record.Exception(() => _validator.Validate(cover));

            Assert.Null(exception);
        }

        [Fact]
        public void ValidCover_DoesNotThrow()
        {
            var start = DateTime.UtcNow.Date.AddDays(1);
            var cover = new Cover
            {
                StartDate = start,
                EndDate = start.AddDays(180),
                Type = CoverType.Yacht
            };

            var exception = Record.Exception(() => _validator.Validate(cover));

            Assert.Null(exception);
        }
    }
}
