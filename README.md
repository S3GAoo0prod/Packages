# Geneirodan.Packages

Libraries and a sample API for CQRS with MediatR, EF Core, ASP.NET Core, and OpenTelemetry.

## Packages

- **Geneirodan.Abstractions** — domain interfaces (`IEntity`, `IUser`), repositories (`IRepository`, `IUnitOfWork`), and mapping.
- **Geneirodan.EntityFrameworkCore** — implementations of `IRepository` and `IUnitOfWork` for EF Core.
- **Geneirodan.MediatR** — pipeline behaviors: logging, authorization, validation, and unhandled exception handling.
- **Geneirodan.AspNetCore** — JWT/auth, `IUser` from HTTP context, localization, health checks, and problem-details exception handler.
- **Geneirodan.Observability** — Serilog and OpenTelemetry wiring.

## Sample API and MediatR pipeline

The **Sample** project (`Sample/Geneirodan.SampleApi`) demonstrates the MediatR pipeline and how to send commands and queries.

### Pipeline order

When you call `ISender.Send(command)` or `ISender.Send(query)`, the pipeline runs in this order:

1. **Logging** — logs the request name and optional user ID.
2. **Authorization** — if the request type has `[Authorize]`, checks that the current user is authenticated and (when `Roles` is set) in one of the allowed roles.
3. **Validation** — runs FluentValidation validators for the request; returns an invalid result without calling the handler if validation fails.
4. **Unhandled exception** — catches exceptions from the handler and rethrows so the ASP.NET Core exception handler can return problem details.
5. **Handler** — the command/query handler runs last.

### Sample commands

- **Command** — no auth; either succeeds or throws when `ShouldFail` is true (used to demonstrate exception handling).
- **ValidatedCommand** — has a FluentValidation validator for `Email`; handler runs only if validation passes.
- **AuthorizedCommand** — requires an authenticated user (`[Authorize]`).
- **AdminCommand** — requires the user to be in the Admin role (`[Authorize(Roles = "Admin")]`).

### Wiring in the Sample

In `Program.cs`, the pipeline and validators are registered with the current assembly:

```csharp
builder.Services.AddMediatRPipeline(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```

To add endpoints that send commands or queries and map `Result`/`Result<T>` to HTTP responses, use `ResultConverter.MapResult` (e.g. in minimal APIs or controllers).

### Running and testing

- Run the Sample API from `Sample/Geneirodan.SampleApi`.
- Integration tests for the pipeline and for EF Core repository/unit-of-work live in `Tests/`.
