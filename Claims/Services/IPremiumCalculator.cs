using Claims.Models;

namespace Claims.Services
{
    /// <summary>
    /// Computes the premium for a Cover, given its type and length.
    /// </summary>
    public interface IPremiumCalculator
    {
        decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType);
    }
}
