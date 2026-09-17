using Claims.Models;

namespace Claims.Services
{
    /// <summary>
    /// Computes the premium for a Cover based on its type and length.
    /// <list type="bullet">
    /// <item>Tier 1 — first 30 days: full day rate.</item>
    /// <item>Tier 2 — next 150 days: discounted (5% Yacht / 2% other types).</item>
    /// <item>Tier 3 — remaining days: discounted further, cumulatively on top of Tier 2
    /// (an additional 3% Yacht / 1% other types, so 8% / 3% off the base rate in total).</item>
    /// </list>
    /// </summary>
    public class PremiumCalculator : IPremiumCalculator
    {
        private const decimal BaseDayRate = 1250m;
        private const int Tier1Days = 30;
        private const int Tier2Days = 150;

        public decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            var totalDays = (int)(endDate.Date - startDate.Date).TotalDays;
            if (totalDays <= 0)
            {
                return 0m;
            }

            var premiumPerDay = BaseDayRate * GetTypeMultiplier(coverType);
            var (tier2Discount, tier3Discount) = GetDiscounts(coverType);

            var tier1Days = Math.Min(totalDays, Tier1Days);
            var tier2ActualDays = Math.Clamp(totalDays - Tier1Days, 0, Tier2Days);
            var tier3ActualDays = Math.Max(totalDays - Tier1Days - Tier2Days, 0);

            return tier1Days * premiumPerDay
                 + tier2ActualDays * premiumPerDay * (1 - tier2Discount)
                 + tier3ActualDays * premiumPerDay * (1 - tier3Discount);
        }

        private static decimal GetTypeMultiplier(CoverType coverType) => coverType switch
        {
            CoverType.Yacht => 1.10m,
            CoverType.PassengerShip => 1.20m,
            CoverType.Tanker => 1.50m,
            _ => 1.30m // ContainerShip, BulkCarrier, and any other type
        };

        private static (decimal Tier2Discount, decimal Tier3Discount) GetDiscounts(CoverType coverType) =>
            coverType == CoverType.Yacht
                ? (0.05m, 0.08m)  // 5%, then an additional 3% => 8% total off base
                : (0.02m, 0.03m); // 2%, then an additional 1% => 3% total off base
    }
}
