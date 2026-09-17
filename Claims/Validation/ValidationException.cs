namespace Claims.Validation
{
    /// <summary>
    /// Thrown when a Claim or Cover fails a business validation rule.
    /// Caught by <see cref="Claims.Middleware.ValidationExceptionMiddleware"/> and turned into
    /// a 400 Bad Request response, so callers get a clear error instead of a 500.
    /// </summary>
    public class ValidationException : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        public ValidationException(IEnumerable<string> errors)
            : base(string.Join(" ", errors))
        {
            Errors = errors.ToList();
        }

        public ValidationException(string error) : this(new[] { error })
        {
        }
    }
}
