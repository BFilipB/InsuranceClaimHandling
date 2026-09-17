using Claims.Models;

namespace Claims.Repositories
{
    /// <summary>
    /// Data access for <see cref="Claim"/> entities. No business logic — that lives in
    /// <see cref="Claims.Services.IClaimsService"/>.
    /// </summary>
    public interface IClaimsRepository
    {
        Task<IEnumerable<Claim>> GetAllAsync();

        Task<Claim?> GetByIdAsync(string id);

        Task AddAsync(Claim claim);

        Task DeleteAsync(string id);
    }
}
