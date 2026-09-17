using Claims.Models;

namespace Claims.Validation
{
    /// <summary>
    /// Validates business rules for a <see cref="Cover"/>.
    /// </summary>
    public interface ICoverValidator
    {
        /// <summary>
        /// Validates a cover. Throws <see cref="ValidationException"/> (with one message per
        /// broken rule) if anything is invalid.
        /// </summary>
        void Validate(Cover cover);
    }
}
