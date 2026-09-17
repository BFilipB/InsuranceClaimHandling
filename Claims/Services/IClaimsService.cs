using Claims.Models;

namespace Claims.Services
{
    /// <summary>
    /// Business logic for Claims: validation, id generation and auditing sit here,
    /// not in the controller or the repository.
    /// </summary>
    public interface IClaimsService
    {
        Task<IEnumerable<Claim>> GetAllAsync();

        Task<Claim?> GetByIdAsync(string id);

        /// <summary>Validates, assigns an id, persists, and audits the new claim.</summary>
        /// <exception cref="Validation.ValidationException">Thrown if a business rule fails.</exception>
        Task<Claim> CreateAsync(Claim claim);

        Task DeleteAsync(string id);
    }
}
