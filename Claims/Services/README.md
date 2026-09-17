# Services

Business logic lives here — the one place that knows the *order* of operations: validate, then compute/assign, then persist, then audit.

`ClaimsService` and `CoversService` are the only classes that call a validator, a repository, and the audit service together. `PremiumCalculator` is deliberately its own class (not a method on `CoversService`) so it can be unit tested with zero dependencies — see `../../Claims.Tests/PremiumCalculatorTests.cs`.

Full reasoning: `../../SOLUTION.md` (Tasks 1 and 5).
