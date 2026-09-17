using Claims.Models;
using Claims.Services;
using Claims.Tests.Fakes;
using Claims.Validation;
using Xunit;

namespace Claims.Tests
{
    public class CoversServiceTests
    {
        private static (FakeCoversRepository Repo, FakeAuditService Audit, CoversService Service) MakeService()
        {
            var repo = new FakeCoversRepository();
            var audit = new FakeAuditService();
            var service = new CoversService(repo, new CoverValidator(), new PremiumCalculator(), audit);
            return (repo, audit, service);
        }

        [Fact]
        public async Task CreateAsync_ValidCover_AssignsIdComputesPremiumPersistsAndAudits()
        {
            var (repo, audit, service) = MakeService();
            var start = DateTime.UtcNow.Date.AddDays(1);

            var cover = new Cover { StartDate = start, EndDate = start.AddDays(30), Type = CoverType.Yacht };
            var created = await service.CreateAsync(cover);

            Assert.False(string.IsNullOrEmpty(created.Id));
            Assert.Equal(30 * 1250m * 1.10m, created.Premium);
            Assert.Single(repo.Covers);
            Assert.Single(audit.CoverAudits);
            Assert.Equal("POST", audit.CoverAudits[0].HttpRequestType);
        }

        [Fact]
        public async Task CreateAsync_StartDateInPast_ThrowsAndDoesNotPersistOrAudit()
        {
            var (repo, audit, service) = MakeService();

            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.Date.AddDays(-5),
                EndDate = DateTime.UtcNow.Date.AddDays(30),
                Type = CoverType.Yacht
            };

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(cover));
            Assert.Empty(repo.Covers);
            Assert.Empty(audit.CoverAudits);
        }

        [Fact]
        public async Task DeleteAsync_AuditsAndRemovesFromRepository()
        {
            var (repo, audit, service) = MakeService();
            repo.Covers.Add(new Cover { Id = "cover-1" });

            await service.DeleteAsync("cover-1");

            Assert.Empty(repo.Covers);
            Assert.Single(audit.CoverAudits);
            Assert.Equal("DELETE", audit.CoverAudits[0].HttpRequestType);
        }

        [Fact]
        public void ComputePremium_DelegatesToPremiumCalculator()
        {
            var (_, _, service) = MakeService();
            var start = new DateTime(2026, 1, 1);

            var result = service.ComputePremium(start, start.AddDays(30), CoverType.Yacht);

            Assert.Equal(30 * 1250m * 1.10m, result);
        }
    }
}
