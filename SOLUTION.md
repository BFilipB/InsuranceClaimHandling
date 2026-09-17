# Solution & Design Decisions

This explains the *why* behind each change, not just the *what* — the code itself is the what. Organized by the 5 tasks in `docs/README.md`. See `CHANGELOG.md` for the exact file-by-file diff.

---

## Task 1 — Layering / SOLID

**Decision:** `Controllers` → `Services` → `Repositories`, everything behind an interface, `ClaimsContext` moved into its own `Data/` folder.

**Why this shape specifically:** the actual complaint in the README was that controllers owned data access *and* business logic *and* constructed their own dependencies (`new Auditer(...)`). Three responsibilities in one class is the textbook SRP violation, and `new`-ing up a dependency instead of receiving it is the textbook DIP violation. A three-layer split with constructor injection fixes both directly — I didn't reach for anything fancier than that.

**Alternative I considered and rejected:** a CQRS/MediatR-style pipeline. It would look more "senior" on paper, but for two entities and four endpoints it adds a dispatch layer and a new dependency to justify a problem this app doesn't have. I'd rather defend a small, obviously-correct design than a bigger one I have to explain away.

**Why `Models/` is its own namespace:** so `Claim`/`Cover` don't implicitly depend on being read from inside `Claims.Controllers` (which is how the original code got away without a `using` — nested-namespace lookup). Explicit `using Claims.Models;` everywhere makes the dependency visible instead of accidental.

---

## Task 2 — Validation

**Decision:** one validator interface per entity (`IClaimValidator`, `ICoverValidator`), called from the Service layer, throwing a custom `ValidationException` that a small piece of middleware turns into `400`.

**Why not `[Range]` / data annotations for the simple rules:** two of the four rules — `Claim.Created` must fall inside its related `Cover`'s period, and (implicitly) the Claim must reference a *real* Cover — need a database lookup the DTO doesn't have access to on its own. Since those two rules can't be attributes, I put *all four* rules in the same kind of place instead of splitting simple ones into attributes and complex ones into services. One validation mental model beats two half-consistent ones.

**Why not FluentValidation:** it's a fine library, but adding a new NuGet dependency to express four rules that fit in ~30 lines of plain C# is more surface area than the problem needs. I'd reach for it the moment rule count or reuse pressure actually justified it.

**Why a custom exception + middleware instead of returning a `Result`/`Either` type from the service:** it keeps the service method signatures honest (`Task<Claim>`, not `Task<Result<Claim>>` everywhere), and the middleware is one file, once. The trade-off — exceptions for control flow — is a real one and I'd say so if asked: for *this* size of app the readability win is worth it.

---

## Task 3 — Async auditing

**Decision:** `AuditService` enqueues onto an in-memory `Channel<AuditEntry>`; a singleton `AuditBackgroundWorker` drains it and does the actual `SaveChangesAsync()`.

**Why in-memory instead of Azure Service Bus / Event Grid:** the README explicitly says in-memory is acceptable, and it means the whole thing still runs with just Docker Desktop — no Azure subscription needed to demo it. `IAuditService` is the seam: swapping the in-memory queue for a real broker later only touches `AuditService`'s internals, not any of its callers.

**Named trade-off, not hidden:** an unbounded in-memory channel means a queued entry is lost if the process crashes before the worker gets to it. For an audit trail that's a real gap in a production system — I'm flagging it rather than pretending the in-memory choice is free.

**Why `IServiceScopeFactory` inside the worker instead of injecting `AuditContext` directly:** `AuditContext` is scoped (one per request) but the worker is a singleton (one for the app's life) — injecting a scoped service straight into a singleton is the "captive dependency" anti-pattern (it'd resolve once, forever, against a context that should be short-lived). Opening a fresh scope per queue entry avoids that.

---

## Task 4 — Tests

**Decision:** hand-rolled in-memory fakes (`FakeClaimsRepository`, `FakeCoversRepository`, `FakeAuditService`) instead of Moq/NSubstitute; unit tests for validators, the premium calculator, and both services; the one pre-existing integration test improved rather than duplicated.

**Why fakes over a mocking library:** no new NuGet dependency, and a fake's behavior is visible in ~15 lines of plain C# rather than hidden behind mock-setup syntax a reviewer has to already know. For three small interfaces this is less code to read overall, not more.

**Why I didn't add more `WebApplicationFactory` integration tests:** the existing one already spins up real SQL Server + Mongo containers via Testcontainers, which is correct but slow. Business rules (validators, premium tiers, service orchestration) are exactly the kind of logic that should be testable *without* paying that Docker cost every run — that's the whole point of Task 1's refactor. I kept exactly one true integration test as a deliberate smoke test, and made it actually assert something (the response body) instead of adding a second, slower copy of the same coverage.

---

## Task 5 — Premium computation

**Decision:** replaced the per-day loop with three explicit, non-overlapping day-count tiers (`Math.Min` / `Math.Clamp` / `Math.Max` off the same `totalDays`), extracted into its own `PremiumCalculator` class.

**Why not patch the existing loop:** the bug wasn't a wrong number, it was a wrong *shape* — three independent `if`s that could all fire for the same day. Tweaking the numbers inside that shape would still leave the double-counting possible for some other input; the tier-based rewrite makes the three ranges mutually exclusive by construction (they're computed from the same `totalDays`, not checked independently per day), so the bug class can't recur.

**Why extract it into its own class:** it turns a private controller method into something you can unit test with zero HTTP, zero DB, zero DI container — which is exactly what the 8 boundary-case tests in `PremiumCalculatorTests.cs` do.

---

## What I deliberately did not do

Naming these myself, rather than waiting for someone else to find them:

- **No Azure-hosting rewrite.** `Program.cs` still starts SQL Server + Mongo via Testcontainers, which is a local-only pattern. The README lists this as optional; I scoped to the local demo.
- **No global exception handler beyond `ValidationException`.** Any other unhandled exception still falls through to ASP.NET Core's default (generic 500). Only `ValidationException` was in scope for what the tasks actually asked for.
- **No new NuGet packages.** Not FluentValidation, not a mocking library, not MediatR/AutoMapper. Everything above uses only what the template already referenced, on the theory that a smaller, fully-explainable diff beats a "more idiomatic-looking" one with dependencies I'd have to justify on the spot.
