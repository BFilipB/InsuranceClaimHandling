# Validation

One validator per entity (`IClaimValidator`, `ICoverValidator`), called from the Service layer before anything is persisted. Both single-field rules (e.g. `DamageCost` limit) and cross-entity rules (e.g. `Claim.Created` must fall inside its `Cover`'s period) live in the same place on purpose — the cross-entity rules need repository data that a plain data-annotation attribute can't reach, so splitting simple rules into attributes and complex rules into services would mean two different validation mental models instead of one.

A failed rule throws `ValidationException`, which `../Middleware/ValidationExceptionMiddleware.cs` turns into a `400` with the list of errors.

Full reasoning: `../../SOLUTION.md` (Task 2).
