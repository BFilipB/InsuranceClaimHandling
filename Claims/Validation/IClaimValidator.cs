using Claims.Models;

namespace Claims.Validation
{
    /// <summary>
    /// Validates business rules for a <see cref="Claim"/>.
    /// </summary>
    public interface IClaimValidator
    {
        /// <summary>
        /// Validates a claim against its related cover. Throws <see cref="ValidationException"/>
        /// (with one message per broken rule) if anything is invalid.
        /// </summary>
        /// <param name="claim">The claim being created.</param>
        /// <param name="relatedCover">
        /// The Cover referenced by <see cref="Claim.CoverId"/>, or null if none was found.
        /// </param>
        void Validate(Claim claim, Cover? relatedCover);
    }
}
