using Claims.Models;

namespace Claims.Services
{
    /// <summary>
    /// Business logic for Covers: validation, premium computation, id generation and
    /// auditing sit here, not in the controller or the repository.
    /// </summary>
    public interface ICoversService
    {
        Task<IEnumerable<Cover>> GetAllAsync();

        Task<Cover?> GetByIdAsync(string id);

        /// <summary>Validates, computes the premium, assigns an id, persists, and audits the new cover.</summary>
        /// <exception cref="Validation.ValidationException">Thrown if a business rule fails.</exception>
        Task<Cover> CreateAsync(Cover cover);

        Task DeleteAsync(string id);

        /// <summary>Computes what the premium would be, without creating a Cover.</summary>
        decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
    }
}
