# ProdeApp Clean Architecture Implementation Plan

Status: Initial architecture slice implemented. Authentication/user-store migration remains deferred.

This plan applies the `clean-architecture` skill to the current ProdeApp solution. It remains the reference workflow for the next implementation slices.

## 1. Current Assessment

Overall assessment: **Critical architectural incompleteness**.

The solution has useful project boundaries, but the Application layer is empty, the API has only a stub controller, authentication is disconnected from persistence, and the current data model has not been reconciled with ASP.NET Identity.

### Current projects

| Project | Intended responsibility | Current state |
|---|---|---|
| `ProdeApp.Domain` | Entities and domain rules | Clean project dependency-wise; entities are mostly anemic persistence models |
| `ProdeApp.Application` | Use cases and ports | Empty placeholders; incorrectly references ASP.NET Identity |
| `ProdeApp.Infrastructure` | EF Core, persistence, Identity, external services | Contains DbContext and migrations; references Application, which is correct for implementing ports |
| `prodeApp` | HTTP interface adapters and composition root | Minimal API setup and a non-API MVC-style auth stub |

### Findings to resolve

1. `ProdeApp.Application.csproj` references `Microsoft.AspNetCore.Identity.EntityFrameworkCore`. This violates the dependency rule.
2. `ProdeApp.Infrastructure.csproj` references `ProdeApp.Application`. This is not a violation. Infrastructure is an outer layer and must reference the inner Application layer to implement its ports.
3. `prodeApp` may reference Application and Infrastructure because it is the composition root. This is the intended dependency direction.
4. There is no implemented use-case boundary. Placeholder `aa.cs` files must be replaced by focused ports, DTOs, and use cases.
5. `AuthController` inherits from `Controller` and returns a view, while the application is configured as an API. Authentication needs an explicit API contract and use cases.
6. `ApplicationUser` does not match `Domain.User`: it uses `IdentityUser<int>.Id`, `Email`, and `DisplayName`, while the domain entity uses `UserId`, `UserName`, and `PasswordHash`. Its mapping methods refer to domain properties that do not exist in the current entity.
7. The existing migration models a custom `Users` table, not the ASP.NET Identity schema. The persistence strategy must be selected before changing authentication or migrations.
8. `AppDbContext` inherits from `DbContext`, so the current Identity type is not registered as an Identity entity store.
9. Package versions are inconsistent with the `net8.0` target: some ASP.NET packages are 8.x while EF Core and Npgsql packages are 9.x. This must be standardized before implementation.
10. The current migration is committed and must not be rewritten casually. Any schema change requires a migration and a database rollout decision.

## 2. Dependency Rule

Source dependencies must point inward toward higher-level policy.

The intended project dependency graph is:

```text
ProdeApp.Domain
        ^
        |
ProdeApp.Application
        ^
        |
ProdeApp.Infrastructure

prodeApp (composition root) --> ProdeApp.Application
prodeApp (composition root) --> ProdeApp.Infrastructure
```

In project-reference terms:

| Project | Allowed references |
|---|---|
| `ProdeApp.Domain` | None |
| `ProdeApp.Application` | `ProdeApp.Domain` only |
| `ProdeApp.Infrastructure` | `ProdeApp.Application`, `ProdeApp.Domain` |
| `prodeApp` | `ProdeApp.Application`, `ProdeApp.Infrastructure` |

The Web API project is both the framework/driver boundary and the initial interface-adapter boundary. A separate `InterfaceAdapters` project is not required at this project size and would add an unnecessary dependency boundary.

## 3. Target Responsibilities

### Domain

Owns stable business concepts and rules:

- User, League, LeagueMembership, Pick, Match, Tournament, and Team
- Domain enums and value objects where a real invariant requires one
- Invariants that must hold regardless of HTTP, EF Core, Identity, or PostgreSQL

Must not reference ASP.NET Core, EF Core, Identity, Npgsql, JWT libraries, or application services.

### Application

Owns application-specific business workflows:

- Use-case input and output models
- Repository and service ports required by use cases
- Authorization and current-user abstractions expressed without framework types
- Validation of workflow rules and orchestration of domain entities
- Transaction boundaries expressed through a small abstraction only when needed

Must not reference ASP.NET Core, EF Core, Identity, database entities, HTTP types, or infrastructure implementations.

### Infrastructure

Owns replaceable technical details:

- `AppDbContext`, EF Core configuration, migrations, and repository implementations
- ASP.NET Identity integration, password hashing, token generation, and claims mapping
- PostgreSQL configuration and external football-data API clients
- Infrastructure dependency-injection registration

Infrastructure may depend on Application ports and Domain entities. It must not move business workflow rules into repositories.

### Web API

Owns external HTTP concerns and application composition:

- Controllers and route definitions
- HTTP request/response DTOs and model validation
- Mapping between HTTP contracts and Application requests/responses
- Authentication middleware and authorization attributes
- Exception-to-HTTP response mapping
- Registration of Application and Infrastructure services

Controllers must remain thin and must not query `AppDbContext` or implement league, pick, scoring, or authentication rules.

## 4. Identity and User Model Decision Gate

Do not implement authentication until this decision is recorded and approved.

### Recommended direction: use ASP.NET Identity for credentials

Use Identity as an Infrastructure concern and keep the Application layer dependent on an abstraction such as a user-authentication port. The Domain user/profile model must be deliberately mapped to the persistence model rather than relying on accidental property compatibility.

Before implementation, decide all of the following:

1. Whether Identity's user table replaces the current custom `Users` table or whether the custom table remains authoritative.
2. Whether `Domain.User` represents the authenticated Identity user, a profile, or a separate domain concept.
3. Which identifier is shared by leagues, memberships, picks, and authentication.
4. Whether existing development data must be migrated.
5. Whether JWT access tokens are required now or can be deferred.

If Identity becomes authoritative, plan a deliberate migration from the existing `Users` schema and update all foreign keys consistently. Do not keep two competing user stores.

## 5. Implementation Sequence

### Phase 0: Establish a clean baseline

No feature work should begin until the baseline is known.

- Inspect the current working tree and preserve unrelated user changes.
- Run the solution build and record all compiler errors.
- Standardize package versions around the selected target framework (`net8.0` unless the project is intentionally upgraded).
- Decide whether the existing migration targets a disposable development database or a database that must be preserved.
- Record the Identity/user-model decision from Section 4.
- Remove placeholder namespace/class names from the implementation backlog; do not delete files until replacement responsibilities are known.

Exit criteria:

- The team has selected one user persistence model.
- Framework package versions are compatible and consistent.
- Existing migration/data preservation requirements are documented.

### Phase 1: Repair project boundaries

Files in scope:

- `ProdeApp.Application/ProdeApp.Application.csproj`
- `ProdeApp.Infrastructure/ProdeApp.Infrastructure.csproj`
- `prodeApp/prodeApp.csproj`
- `prodeApp/prodeApp.sln`

Actions:

1. Remove Identity and other framework packages from Application.
2. Keep the Infrastructure reference to Application; it is required for adapter implementations.
3. Keep the Web API references to Application and Infrastructure as the composition root.
4. Add only the packages needed by each project.
5. Keep Domain free of package and project references.

Exit criteria:

- Domain has no external references.
- Application references Domain only.
- Infrastructure references Application and Domain.
- Web API references Application and Infrastructure.
- No project-reference cycle exists.

### Phase 2: Define Application ports and contracts

Create focused interfaces based on actual use cases, not one interface per database table by default.

Initial port groups:

- Authentication: register, authenticate, current-user identity, and token issuance abstractions
- Leagues: create, join, leave, retrieve, and standings queries
- Picks: create/update prediction and retrieve predictions
- Matches: retrieve scheduled matches and record results
- Tournaments: retrieve active tournaments and manage tournament data if required

For each port:

1. Define only methods required by a use case.
2. Use cancellation tokens for asynchronous I/O.
3. Use Application request/response records or DTOs, not EF entities or HTTP types.
4. Define explicit not-found, validation, conflict, and authorization outcomes.
5. Keep repository interfaces small; do not create a generic repository without a concrete use-case need.

Create Application use cases in vertical slices rather than large entity services. The first slice should be authentication or league creation, followed by the smallest end-to-end league/pick flow.

Exit criteria:

- A use case can be unit tested with in-memory/fake ports.
- No Application type requires ASP.NET Core, EF Core, Identity, or Infrastructure.
- Boundary contracts do not expose Domain entity graphs.

### Phase 3: Make domain invariants explicit

Review each domain concept before writing repositories.

At minimum, decide and test rules for:

- A private league requiring a join code
- League owner and membership constraints
- Maximum league membership
- Whether a user may submit one pick per user/match/league
- Whether picks are blocked after match start or completion
- Valid goal values and result state transitions
- Scoring rules and when points are awarded
- Tournament and league finish dates

Move a rule into Domain only when it is a business invariant independent of a specific use-case workflow. Keep orchestration in Application.

Exit criteria:

- Invalid state transitions are rejected consistently.
- The unique pick constraint is enforced both by Domain/Application behavior and the database constraint.
- Domain tests do not require a database or web host.

### Phase 4: Implement Infrastructure adapters

Files and folders:

- `ProdeApp.Infrastructure/Persistance/AppDbContext.cs`
- `ProdeApp.Infrastructure/Persistance/Repositories/`
- `ProdeApp.Infrastructure/Identity/`
- `ProdeApp.Infrastructure/ExternalApis/`
- `ProdeApp.Infrastructure/DependencyInjection.cs`

Actions:

1. Implement Application repository ports with EF Core queries.
2. Keep eager loading and query optimization in Infrastructure.
3. Add EF configuration for relationships, indexes, required fields, and delete behavior.
4. Resolve the custom User versus Identity model before adding authentication repositories.
5. Implement current-user, password, token, and role ports behind Infrastructure adapters.
6. Add external API clients only behind Application-owned gateway ports.
7. Register all adapters in one Infrastructure extension method.

Migration rules:

- Never edit an applied migration to change live schema.
- Add a new migration for approved schema changes.
- Review generated SQL and foreign-key effects before applying it.
- Preserve or explicitly migrate existing user and domain data.

Exit criteria:

- Infrastructure implementations compile against Application ports.
- Repositories contain data access only.
- Identity uses one authoritative user store.
- Database schema and model snapshot agree.

### Phase 5: Implement Web API adapters

Files and folders:

- `prodeApp/API/Controllers/`
- `prodeApp/API/Contracts/` or `prodeApp/API/Dto/`
- `prodeApp/API/Middleware/`
- `prodeApp/Program.cs`

Actions:

1. Replace the MVC-style `AuthController` with API endpoints backed by Application use cases.
2. Add controllers for authentication, leagues, matches, picks, and tournaments only as use cases become available.
3. Keep request and response contracts separate from Domain entities.
4. Map route, query, and body input into Application requests.
5. Map Application outcomes into consistent HTTP status codes.
6. Add authentication and authorization at the HTTP boundary while keeping authorization decisions required by workflows in Application.
7. Centralize exception and validation response handling.
8. Keep `Program.cs` as the composition root: configuration, middleware, Application registration, Infrastructure registration, and controllers.

Exit criteria:

- Controllers contain no EF Core or business-rule code.
- No Domain entity is serialized directly as an API response.
- API behavior is covered by endpoint tests for implemented slices.

### Phase 6: Verify and incrementally expand

Implement one vertical slice completely before adding every entity repository.

Recommended slice order:

1. Register/login/current user
2. Create and retrieve leagues
3. Join/leave league
4. Retrieve matches and submit/update picks
5. Calculate and retrieve standings
6. Tournament administration and external match synchronization

For every slice:

1. Add or update Domain tests for invariants.
2. Add Application unit tests using fake ports.
3. Add Infrastructure integration tests against a disposable PostgreSQL database where persistence behavior matters.
4. Add Web API tests for authentication, validation, status codes, and response contracts.
5. Build the full solution before moving to the next slice.

## 6. Verification Checklist

### Dependency checks

- [ ] Domain has no project or framework dependencies.
- [ ] Application references Domain only.
- [ ] Application has no ASP.NET Core, EF Core, Identity, JWT, or PostgreSQL package.
- [ ] Infrastructure references Application and implements its ports.
- [ ] Web API is the composition root.
- [ ] No project dependency cycle exists.

### Boundary checks

- [ ] Use cases do not accept `HttpContext`, `IActionResult`, `DbContext`, or Identity types.
- [ ] Controllers do not access `DbContext` directly.
- [ ] Domain entities are not used as API request/response contracts.
- [ ] Infrastructure types do not appear in Application contracts.
- [ ] External API response models are translated inside Infrastructure.

### Behavior checks

- [ ] Authentication uses one authoritative user store.
- [ ] League membership and privacy rules are tested.
- [ ] Pick timing and uniqueness rules are tested.
- [ ] Match result and scoring rules are tested.
- [ ] Authorization is tested for owner/member/admin paths.

### Build and migration checks

- [ ] `dotnet build prodeApp/prodeApp.sln` succeeds.
- [ ] Unit and integration tests pass.
- [ ] EF model snapshot matches the intended schema.
- [ ] New migrations are reviewed before applying them.
- [ ] No secrets are committed in configuration.

## 7. Explicit Non-Goals

Do not do these as part of the first architecture pass:

- Add a separate InterfaceAdapters project solely for folder purity.
- Add generic repositories or generic service bases without a current use case.
- Add CQRS, MediatR, event sourcing, or a message bus without a concrete requirement.
- Duplicate every DTO across every layer when a single stable Application contract is sufficient.
- Rewrite existing migrations without a data-preservation decision.
- Add domain abstractions for framework behavior that Application does not actually need.

## 8. Definition Of Done

The architecture work is complete when:

1. The project dependency graph follows the Dependency Rule.
2. Application use cases are implemented and testable without infrastructure.
3. Infrastructure implements ports without leaking persistence concerns inward.
4. The API exposes implemented use cases through thin controllers and explicit contracts.
5. Identity and the user schema use one deliberate, tested model.
6. Domain invariants and use-case behavior have automated tests.
7. The solution builds cleanly and database changes are represented by reviewed migrations.

*This document is the implementation workflow. The next execution step may begin with Phase 0, but must not modify source code until the Identity/user-model decision is resolved.*
