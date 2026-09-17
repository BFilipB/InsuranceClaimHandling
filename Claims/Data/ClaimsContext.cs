using Claims.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Data
{
    /// <summary>
    /// EF Core context over MongoDB for the Claims and Covers collections.
    /// Only data access lives here — no business logic. See <see cref="Claims.Repositories"/>
    /// for query methods and <see cref="Claims.Services"/> for business rules.
    /// </summary>
    public class ClaimsContext : DbContext
    {
        public DbSet<Claim> Claims { get; init; } = null!;
        public DbSet<Cover> Covers { get; init; } = null!;

        public ClaimsContext(DbContextOptions<ClaimsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Claim>().ToCollection("claims");
            modelBuilder.Entity<Cover>().ToCollection("covers");
        }
    }
}
