namespace Claims.Auditing
{
    /// <summary>
    /// A pending audit write, queued in-memory until <see cref="AuditBackgroundWorker"/> persists it.
    /// </summary>
    public record AuditEntry(string EntityId, string HttpRequestType, AuditEntryType Type, DateTime Created);

    public enum AuditEntryType
    {
        Claim,
        Cover
    }
}
