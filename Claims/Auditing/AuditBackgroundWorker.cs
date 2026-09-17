namespace Claims.Auditing
{
    /// <summary>
    /// Runs for the lifetime of the app, reading queued audit entries and persisting them to
    /// <see cref="AuditContext"/>. Uses <see cref="IServiceScopeFactory"/> because AuditContext
    /// is scoped (one per request) but this worker is a singleton (one for the whole app).
    /// </summary>
    public class AuditBackgroundWorker : BackgroundService
    {
        private readonly AuditQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuditBackgroundWorker> _logger;

        public AuditBackgroundWorker(AuditQueue queue, IServiceScopeFactory scopeFactory, ILogger<AuditBackgroundWorker> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var entry in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var auditContext = scope.ServiceProvider.GetRequiredService<AuditContext>();

                    if (entry.Type == AuditEntryType.Claim)
                    {
                        auditContext.Add(new ClaimAudit
                        {
                            ClaimId = entry.EntityId,
                            HttpRequestType = entry.HttpRequestType,
                            Created = entry.Created
                        });
                    }
                    else
                    {
                        auditContext.Add(new CoverAudit
                        {
                            CoverId = entry.EntityId,
                            HttpRequestType = entry.HttpRequestType,
                            Created = entry.Created
                        });
                    }

                    await auditContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    // Don't let one bad entry crash the worker — log it and keep processing the queue.
                    _logger.LogError(ex, "Failed to persist audit entry for {EntityId}", entry.EntityId);
                }
            }
        }
    }
}
