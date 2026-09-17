using Claims.Auditing;

namespace Claims.Tests.Fakes
{
    /// <summary>
    /// Records calls instead of enqueueing/persisting anything, so tests can assert that
    /// auditing happened without needing the real queue or database.
    /// </summary>
    public class FakeAuditService : IAuditService
    {
        public List<(string Id, string HttpRequestType)> ClaimAudits { get; } = new();
        public List<(string Id, string HttpRequestType)> CoverAudits { get; } = new();

        public Task AuditClaimAsync(string id, string httpRequestType)
        {
            ClaimAudits.Add((id, httpRequestType));
            return Task.CompletedTask;
        }

        public Task AuditCoverAsync(string id, string httpRequestType)
        {
            CoverAudits.Add((id, httpRequestType));
            return Task.CompletedTask;
        }
    }
}
