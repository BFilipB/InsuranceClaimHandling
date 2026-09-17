# Data

`ClaimsContext` — the EF Core context over the MongoDB `claims` and `covers` collections. Originally declared *inside* `ClaimsController.cs`; moved here so it's a normal, independently-testable/injectable dependency instead of something only the controller could see.

Only `Repositories/` (`../Repositories/`) talks to this context directly.
