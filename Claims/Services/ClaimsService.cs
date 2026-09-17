using Claims.Auditing;
using Claims.Models;
using Claims.Repositories;
using Claims.Validation;

namespace Claims.Services
{
    /// <inheritdoc cref="IClaimsService"/>
    public class ClaimsService : IClaimsService
    {
        private readonly IClaimsRepository _claimsRepository;
        private readonly ICoversRepository _coversRepository;
        private readonly IClaimValidator _claimValidator;
        private readonly IAuditService _auditService;

        public ClaimsService(
            IClaimsRepository claimsRepository,
            ICoversRepository coversRepository,
            IClaimValidator claimValidator,
            IAuditService auditService)
        {
            _claimsRepository = claimsRepository;
            _coversRepository = coversRepository;
            _claimValidator = claimValidator;
            _auditService = auditService;
        }

        public Task<IEnumerable<Claim>> GetAllAsync() => _claimsRepository.GetAllAsync();

        public Task<Claim?> GetByIdAsync(string id) => _claimsRepository.GetByIdAsync(id);

        public async Task<Claim> CreateAsync(Claim claim)
        {
            var relatedCover = await _coversRepository.GetByIdAsync(claim.CoverId);
            _claimValidator.Validate(claim, relatedCover); // throws ValidationException if invalid

            claim.Id = Guid.NewGuid().ToString();
            await _claimsRepository.AddAsync(claim);

            // Non-blocking: this only enqueues the audit entry, it doesn't wait on the audit DB write.
            await _auditService.AuditClaimAsync(claim.Id, "POST");

            return claim;
        }

        public async Task DeleteAsync(string id)
        {
            await _auditService.AuditClaimAsync(id, "DELETE");
            await _claimsRepository.DeleteAsync(id);
        }
    }
}
