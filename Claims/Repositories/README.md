# Repositories

Data access only — nothing here knows about validation, auditing, or HTTP.

Each entity gets one interface + one implementation (`IClaimsRepository`/`ClaimsRepository`, `ICoversRepository`/`CoversRepository`) so the Service layer (see `../Services/README.md`) depends on an abstraction it can fake in tests, instead of on `ClaimsContext` directly. See `../../SOLUTION.md` (Task 1) for the full reasoning.
