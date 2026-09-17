using Claims.Models;

namespace Claims.Repositories
{
    /// <summary>
    /// Data access for <see cref="Cover"/> entities. No business logic — that lives in
    /// <see cref="Claims.Services.ICoversService"/>.
    /// </summary>
    public interface ICoversRepository
    {
        Task<IEnumerable<Cover>> GetAllAsync();

        Task<Cover?> GetByIdAsync(string id);

        Task AddAsync(Cover cover);

        Task DeleteAsync(string id);
    }
}
