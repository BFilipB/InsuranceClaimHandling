# Per-commit, per-file change log

Pulled directly from `git log --name-status --numstat` on the rebuilt repo — the file list and +/- line counts are real git data, not written from memory. The one-line "what changed" column is my own annotation.


## Commit 0 — `145be82` — Initial commit

_This is the interviewer's original template, cloned as-is — nothing in this commit was written by me. Listed here only so the diff of every later commit is readable against a known baseline._

<details><summary>21 files, unmodified template — click to expand</summary>

| File | Change | Lines | What changed |
|---|---|---|---|
| `.gitignore` | Added | +301/-0 |  |
| `Claims.Tests/Claims.Tests.csproj` | Added | +24/-0 |  |
| `Claims.Tests/ClaimsControllerTests.cs` | Added | +25/-0 |  |
| `Claims.sln` | Added | +42/-0 |  |
| `Claims/Auditing/AuditContext.cs` | Added | +13/-0 |  |
| `Claims/Auditing/Auditer.cs` | Added | +38/-0 |  |
| `Claims/Auditing/ClaimAudit.cs` | Added | +13/-0 |  |
| `Claims/Auditing/CoverAudit.cs` | Added | +13/-0 |  |
| `Claims/Claim.cs` | Added | +35/-0 |  |
| `Claims/Claims.csproj` | Added | +22/-0 |  |
| `Claims/Controllers/ClaimsController.cs` | Added | +99/-0 |  |
| `Claims/Controllers/CoversController.cs` | Added | +98/-0 |  |
| `Claims/Cover.cs` | Added | +32/-0 |  |
| `Claims/Migrations/20220728113129_1stMigration.Designer.cs` | Added | +53/-0 |  |
| `Claims/Migrations/20220728113129_1stMigration.cs` | Added | +52/-0 |  |
| `Claims/Migrations/AuditContextModelSnapshot.cs` | Added | +75/-0 |  |
| `Claims/Program.cs` | Added | +72/-0 |  |
| `Claims/Properties/launchSettings.json` | Added | +31/-0 |  |
| `Claims/appsettings.json` | Added | +12/-0 |  |
| `docs/CODEOWNERS` | Added | +4/-0 |  |
| `docs/README.md` | Added | +57/-0 |  |
</details>


## Commit 1 — `947bfd6` — refactor(task-1): extract Models, Data and Repository layers out of the controllers

| File | Change | Lines | What changed |
|---|---|---|---|
| `Claims/Cover.cs` | Deleted | +0/-32 | Deleted — content moved to Models/Cover.cs (Task 1). |
| `Claims/Models/Claim.cs` | Renamed (Claims/Claim.cs → Claims/Models/Claim.cs) | +7/-5 | Renamed from Claims/Claim.cs. Only the namespace changed (Claims -> Claims.Models); fields/enum untouched. |
| `Claims/Models/Cover.cs` | Added | +36/-0 | New location for the old Claims/Cover.cs. Only the namespace changed; fields/enum untouched. |
| `Claims/Repositories/ClaimsRepository.cs` | Added | +45/-0 | New class implementing IClaimsRepository, wrapping ClaimsContext — this is the only place that touches the Mongo Claims collection now. |
| `Claims/Repositories/CoversRepository.cs` | Added | +45/-0 | New class implementing ICoversRepository, wrapping ClaimsContext — this is the only place that touches the Mongo Covers collection now. |
| `Claims/Repositories/IClaimsRepository.cs` | Added | +19/-0 | New interface: GetAllAsync, GetByIdAsync, AddAsync, DeleteAsync for Claim. |
| `Claims/Repositories/ICoversRepository.cs` | Added | +19/-0 | New interface: GetAllAsync, GetByIdAsync, AddAsync, DeleteAsync for Cover. |

## Commit 2 — `d278c4f` — feat(task-2): add Claim and Cover business validation rules

| File | Change | Lines | What changed |
|---|---|---|---|
| `Claims/Validation/ClaimValidator.cs` | Added | +36/-0 | New class: throws if DamageCost > 100,000, or if Created falls outside the related Cover's StartDate/EndDate. |
| `Claims/Validation/CoverValidator.cs` | Added | +33/-0 | New class: throws if StartDate is in the past, or if the period exceeds 365 days. |
| `Claims/Validation/IClaimValidator.cs` | Added | +20/-0 | New interface: Validate(Claim, Cover?). |
| `Claims/Validation/ICoverValidator.cs` | Added | +16/-0 | New interface: Validate(Cover). |
| `Claims/Validation/ValidationException.cs` | Added | +22/-0 | New exception type carrying a list of rule-violation messages; caught later by the middleware (commit 6). |

## Commit 3 — `719bf00` — fix(task-5): correct and simplify the Cover premium computation

| File | Change | Lines | What changed |
|---|---|---|---|
| `Claims/Services/IPremiumCalculator.cs` | Added | +12/-0 | New interface: Compute(startDate, endDate, coverType) -> decimal. |
| `Claims/Services/PremiumCalculator.cs` | Added | +53/-0 | New class. Replaces the old per-day loop (which double-counted days) with 3 non-overlapping day-count tiers via Math.Min/Clamp/Max. This is the Task 5 bug fix. |

## Commit 4 — `af4aece` — refactor(task-1): introduce the Service layer (ClaimsService/CoversService)

| File | Change | Lines | What changed |
|---|---|---|---|
| `Claims/Services/ClaimsService.cs` | Added | +52/-0 | New class: looks up the related Cover, calls IClaimValidator, assigns a new Guid, calls IClaimsRepository, then calls IAuditService. |
| `Claims/Services/CoversService.cs` | Added | +56/-0 | New class: calls ICoverValidator, computes the premium via IPremiumCalculator, assigns a new Guid, calls ICoversRepository, then calls IAuditService. |
| `Claims/Services/IClaimsService.cs` | Added | +21/-0 | New interface: GetAllAsync, GetByIdAsync, CreateAsync, DeleteAsync for Claim — the business-logic contract the controller will depend on. |
| `Claims/Services/ICoversService.cs` | Added | +24/-0 | New interface: GetAllAsync, GetByIdAsync, CreateAsync, DeleteAsync, ComputePremium for Cover. |

## Commit 5 — `77f3a6b` — feat(task-3): make claim/cover auditing asynchronous

| File | Change | Lines | What changed |
|---|---|---|---|
| `Claims/Auditing/AuditBackgroundWorker.cs` | Added | +59/-0 | New BackgroundService: loops over the queue for the app's lifetime, opens a fresh DI scope per entry, and does the actual AuditContext.SaveChangesAsync(). |
| `Claims/Auditing/AuditEntry.cs` | Added | +13/-0 | New record: one pending audit write (EntityId, HttpRequestType, Type, Created) + AuditEntryType enum. |
| `Claims/Auditing/AuditQueue.cs` | Added | +18/-0 | New singleton: wraps an unbounded Channel<AuditEntry> so the request thread (producer) and the worker (consumer) share one queue. |
| `Claims/Auditing/AuditService.cs` | Added | +23/-0 | New class: just calls AuditQueue.EnqueueAsync and returns — this is what makes create/delete requests non-blocking (Task 3). |
| `Claims/Auditing/Auditer.cs` | Deleted | +0/-38 | Deleted. This was the old class that called SaveChanges() synchronously on the request thread — fully replaced by the 5 files above. |
| `Claims/Auditing/IAuditService.cs` | Added | +13/-0 | New interface: AuditClaimAsync(id, httpRequestType), AuditCoverAsync(id, httpRequestType). |

## Commit 6 — `a9787f0` — refactor(task-1): slim controllers down to HTTP concerns only

| File | Change | Lines | What changed |
|---|---|---|---|
| `Claims/Controllers/ClaimsController.cs` | Modified | +26/-69 | Rewritten. No longer declares its own ClaimsContext (moved to Data/ in commit 1) or 'new Auditer(...)' — now just calls IClaimsService and maps the result to an ActionResult. |
| `Claims/Controllers/CoversController.cs` | Modified | +41/-76 | Rewritten. Same pattern: calls ICoversService only. Also: ComputePremiumAsync (async with no await — a compiler warning) renamed to a plain synchronous ComputePremium. |
| `Claims/Middleware/ValidationExceptionMiddleware.cs` | Added | +35/-0 | New middleware: catches ValidationException thrown anywhere downstream and turns it into a 400 with { errors: [...] } instead of a 500. |

## Commit 7 — `e9aef14` — chore(task-1): wire the new layers into dependency injection

| File | Change | Lines | What changed |
|---|---|---|---|
| `Claims/Program.cs` | Modified | +30/-1 | Modified. Added AddScoped/AddSingleton/AddHostedService registrations for every interface introduced in commits 1-6, plus app.UseMiddleware<ValidationExceptionMiddleware>() in the request pipeline. Nothing about the Testcontainers/SQL/Mongo startup logic changed. |

## Commit 8 — `a170e0e` — test(task-4): add unit tests for validators, premium calculator and services

| File | Change | Lines | What changed |
|---|---|---|---|
| `Claims.Tests/ClaimValidatorTests.cs` | Added | +76/-0 | New: 6 tests — one pass/fail pair for each of the 2 Claim rules, plus the missing-related-Cover case. |
| `Claims.Tests/ClaimsControllerTests.cs` | Modified | +9/-3 | Modified. Get_Claims now deserializes the response body and asserts it's a non-null Claim list — the original only checked for 200 OK. |
| `Claims.Tests/ClaimsServiceTests.cs` | Added | +84/-0 | New: 4 tests exercising ClaimsService end-to-end against the fakes (create success, create validation failure, unknown cover, delete). |
| `Claims.Tests/CoverValidatorTests.cs` | Added | +85/-0 | New: 5 tests — one pass/fail pair for each of the 2 Cover rules, plus a fully-valid case. |
| `Claims.Tests/CoversServiceTests.cs` | Added | +76/-0 | New: 4 tests exercising CoversService end-to-end against the fakes (create success, create validation failure, delete, ComputePremium delegation). |
| `Claims.Tests/Fakes/FakeAuditService.cs` | Added | +26/-0 | New: in-memory IAuditService that just records calls, so tests can assert 'was this audited?'. |
| `Claims.Tests/Fakes/FakeClaimsRepository.cs` | Added | +34/-0 | New: in-memory IClaimsRepository backed by a List<Claim>, used so service tests don't need a real DB. |
| `Claims.Tests/Fakes/FakeCoversRepository.cs` | Added | +35/-0 | New: in-memory ICoversRepository backed by a List<Cover>. |
| `Claims.Tests/PremiumCalculatorTests.cs` | Added | +117/-0 | New: 8 tests — all 5 CoverTypes at exactly 30 days, Yacht and non-Yacht at exactly 180 and >180 days, plus zero/negative length. |