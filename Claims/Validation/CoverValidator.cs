using Claims.Models;

namespace Claims.Validation
{
    /// <inheritdoc cref="ICoverValidator"/>
    public class CoverValidator : ICoverValidator
    {
        private const int MaxInsurancePeriodDays = 365;

        public void Validate(Cover cover)
        {
            var errors = new List<string>();

            // Rule: StartDate cannot be in the past
            if (cover.StartDate.Date < DateTime.UtcNow.Date)
            {
                errors.Add("Cover StartDate cannot be in the past.");
            }

            // Rule: total insurance period cannot exceed 1 year
            var totalDays = (cover.EndDate.Date - cover.StartDate.Date).TotalDays;
            if (totalDays > MaxInsurancePeriodDays)
            {
                errors.Add($"Total insurance period cannot exceed {MaxInsurancePeriodDays} days.");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
        }
    }
}
