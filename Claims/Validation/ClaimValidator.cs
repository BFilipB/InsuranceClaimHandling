using Claims.Models;

namespace Claims.Validation
{
    /// <inheritdoc cref="IClaimValidator"/>
    public class ClaimValidator : IClaimValidator
    {
        private const decimal MaxDamageCost = 100_000m;

        public void Validate(Claim claim, Cover? relatedCover)
        {
            var errors = new List<string>();

            // Rule: DamageCost cannot exceed 100,000
            if (claim.DamageCost > MaxDamageCost)
            {
                errors.Add($"DamageCost cannot exceed {MaxDamageCost:N0}.");
            }

            if (relatedCover is null)
            {
                errors.Add($"Claim references CoverId '{claim.CoverId}' which does not exist.");
            }
            else if (claim.Created.Date < relatedCover.StartDate.Date || claim.Created.Date > relatedCover.EndDate.Date)
            {
                // Rule: Created date must be within the period of the related Cover
                errors.Add("Claim Created date must fall within the related Cover's period.");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
        }
    }
}
