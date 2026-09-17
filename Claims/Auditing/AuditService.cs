namespace Claims.Auditing
{
    /// <summary>
    /// Enqueues audit entries instead of writing to the database directly, so create/delete
    /// requests don't block on the audit DB write. <see cref="AuditBackgroundWorker"/> does the
    /// actual persistence off the request path. This is what fixes Task 3.
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly AuditQueue _queue;

        public AuditService(AuditQueue queue)
        {
            _queue = queue;
        }

        public Task AuditClaimAsync(string id, string httpRequestType) =>
            _queue.EnqueueAsync(new AuditEntry(id, httpRequestType, AuditEntryType.Claim, DateTime.Now)).AsTask();

        public Task AuditCoverAsync(string id, string httpRequestType) =>
            _queue.EnqueueAsync(new AuditEntry(id, httpRequestType, AuditEntryType.Cover, DateTime.Now)).AsTask();
    }
}
