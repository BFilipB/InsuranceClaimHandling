using Claims.Models;
using Claims.Repositories;

namespace Claims.Tests.Fakes
{
    /// <summary>
    /// In-memory stand-in for <see cref="ICoversRepository"/>, used to unit test
    /// <see cref="Claims.Services.ClaimsService"/> and <see cref="Claims.Services.CoversService"/>
    /// without a real database.
    /// </summary>
    public class FakeCoversRepository : ICoversRepository
    {
        public List<Cover> Covers { get; } = new();

        public Task<IEnumerable<Cover>> GetAllAsync() => Task.FromResult(Covers.AsEnumerable());

        public Task<Cover?> GetByIdAsync(string id) => Task.FromResult(Covers.SingleOrDefault(c => c.Id == id));

        public Task AddAsync(Cover cover)
        {
            Covers.Add(cover);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var cover = Covers.SingleOrDefault(c => c.Id == id);
            if (cover is not null)
            {
                Covers.Remove(cover);
            }
            return Task.CompletedTask;
        }
    }
}
