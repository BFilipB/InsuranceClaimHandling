# Auditing

`AuditService` doesn't write to the database — it enqueues an `AuditEntry` onto `AuditQueue` (an in-memory `Channel<T>`) and returns immediately. `AuditBackgroundWorker` is a separate, long-running `BackgroundService` that drains that queue and does the actual `SaveChangesAsync()` against `AuditContext`, off the HTTP request path entirely. That's what makes claim/cover creation and deletion non-blocking.

`AuditContext`, `ClaimAudit`, and `CoverAudit` are unchanged from the original template — only the *writing path* changed, not the audit schema.

**Known trade-off:** the queue is in-memory, so a pending entry is lost if the process crashes before the worker persists it. Swapping it for a real message broker (Azure Service Bus, Event Grid) later only means rewriting `AuditService`'s internals — `IAuditService` stays the same, so nothing that calls it needs to change.

Full reasoning: `../../SOLUTION.md` (Task 3).
