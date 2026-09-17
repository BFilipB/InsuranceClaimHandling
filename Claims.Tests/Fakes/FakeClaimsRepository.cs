using Claims.Models;
using Claims.Repositories;

namespace Claims.Tests.Fakes
{
    /// <summary>
    /// In-memory stand-in for <see cref="IClaimsRepository"/>, used to unit test
    /// <see cref="Claims.Services.ClaimsService"/> without a real database.
    /// </summary>
    public class FakeClaimsRepository : IClaimsRepository
    {
        public List<Claim> Claims { get; } = new();

        public Task<IEnumerable<Claim>> GetAllAsync() => Task.FromResult(Claims.AsEnumerable());

        public Task<Claim?> GetByIdAsync(string id) => Task.FromResult(Claims.SingleOrDefault(c => c.Id == id));

        public Task AddAsync(Claim claim)
        {
            Claims.Add(claim);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var claim = Claims.SingleOrDefault(c => c.Id == id);
            if (claim is not null)
            {
                Claims.Remove(claim);
            }
            return Task.CompletedTask;
        }
    }
}
