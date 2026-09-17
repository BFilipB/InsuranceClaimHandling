using Claims.Auditing;
using Claims.Models;
using Claims.Repositories;
using Claims.Validation;

namespace Claims.Services
{
    /// <inheritdoc cref="ICoversService"/>
    public class CoversService : ICoversService
    {
        private readonly ICoversRepository _coversRepository;
        private readonly ICoverValidator _coverValidator;
        private readonly IPremiumCalculator _premiumCalculator;
        private readonly IAuditService _auditService;

        public CoversService(
            ICoversRepository coversRepository,
            ICoverValidator coverValidator,
            IPremiumCalculator premiumCalculator,
            IAuditService auditService)
        {
            _coversRepository = coversRepository;
            _coverValidator = coverValidator;
            _premiumCalculator = premiumCalculator;
            _auditService = auditService;
        }

        public Task<IEnumerable<Cover>> GetAllAsync() => _coversRepository.GetAllAsync();

        public Task<Cover?> GetByIdAsync(string id) => _coversRepository.GetByIdAsync(id);

        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType) =>
            _premiumCalculator.Compute(startDate, endDate, coverType);

        public async Task<Cover> CreateAsync(Cover cover)
        {
            _coverValidator.Validate(cover); // throws ValidationException if invalid

            cover.Id = Guid.NewGuid().ToString();
            cover.Premium = _premiumCalculator.Compute(cover.StartDate, cover.EndDate, cover.Type);

            await _coversRepository.AddAsync(cover);

            // Non-blocking: this only enqueues the audit entry, it doesn't wait on the audit DB write.
            await _auditService.AuditCoverAsync(cover.Id, "POST");

            return cover;
        }

        public async Task DeleteAsync(string id)
        {
            await _auditService.AuditCoverAsync(id, "DELETE");
            await _coversRepository.DeleteAsync(id);
        }
    }
}
