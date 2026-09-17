namespace Claims.Auditing
{
    /// <summary>
    /// Records that a Claim or Cover was created or deleted. Implementations must not block
    /// the calling request on the actual database write.
    /// </summary>
    public interface IAuditService
    {
        Task AuditClaimAsync(string id, string httpRequestType);

        Task AuditCoverAsync(string id, string httpRequestType);
    }
}
