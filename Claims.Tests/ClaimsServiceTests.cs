using Claims.Models;
using Claims.Services;
using Claims.Tests.Fakes;
using Claims.Validation;
using Xunit;

namespace Claims.Tests
{
    public class ClaimsServiceTests
    {
        private static (FakeClaimsRepository ClaimsRepo, FakeCoversRepository CoversRepo, FakeAuditService Audit, ClaimsService Service) MakeService()
        {
            var claimsRepo = new FakeClaimsRepository();
            var coversRepo = new FakeCoversRepository();
            var audit = new FakeAuditService();
            var service = new ClaimsService(claimsRepo, coversRepo, new ClaimValidator(), audit);
            return (claimsRepo, coversRepo, audit, service);
        }

        [Fact]
        public async Task CreateAsync_ValidClaim_AssignsIdPersistsAndAudits()
        {
            var (claimsRepo, coversRepo, audit, service) = MakeService();
            coversRepo.Covers.Add(new Cover
            {
                Id = "cover-1",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 6, 1),
                Type = CoverType.Yacht
            });

            var claim = new Claim { CoverId = "cover-1", DamageCost = 500m, Created = new DateTime(2026, 2, 1) };
            var created = await service.CreateAsync(claim);

            Assert.False(string.IsNullOrEmpty(created.Id));
            Assert.Single(claimsRepo.Claims);
            Assert.Single(audit.ClaimAudits);
            Assert.Equal("POST", audit.ClaimAudits[0].HttpRequestType);
        }

        [Fact]
        public async Task CreateAsync_DamageCostTooHigh_ThrowsAndDoesNotPersistOrAudit()
        {
            var (claimsRepo, coversRepo, audit, service) = MakeService();
            coversRepo.Covers.Add(new Cover
            {
                Id = "cover-1",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 6, 1),
                Type = CoverType.Yacht
            });

            var claim = new Claim { CoverId = "cover-1", DamageCost = 999_999m, Created = new DateTime(2026, 2, 1) };

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(claim));
            Assert.Empty(claimsRepo.Claims);
            Assert.Empty(audit.ClaimAudits);
        }

        [Fact]
        public async Task CreateAsync_UnknownCover_ThrowsAndDoesNotPersist()
        {
            var (claimsRepo, _, _, service) = MakeService();

            var claim = new Claim { CoverId = "does-not-exist", DamageCost = 100m, Created = DateTime.UtcNow };

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(claim));
            Assert.Empty(claimsRepo.Claims);
        }

        [Fact]
        public async Task DeleteAsync_AuditsAndRemovesFromRepository()
        {
            var (claimsRepo, _, audit, service) = MakeService();
            claimsRepo.Claims.Add(new Claim { Id = "claim-1", CoverId = "cover-1" });

            await service.DeleteAsync("claim-1");

            Assert.Empty(claimsRepo.Claims);
            Assert.Single(audit.ClaimAudits);
            Assert.Equal("DELETE", audit.ClaimAudits[0].HttpRequestType);
        }
    }
}
