# Repository Guidelines

## Project Structure & Module Organization

The solution lives in `NationalTeamManager/` and contains the ASP.NET API (`NationalTeamManager.Api`), application queries (`NationalTeamManager.Application`), domain entities (`NationalTeamManager.Domain`), persistence and services (`NationalTeamManager.Infrastructure`), a data importer, and the React client (`NationalTeamManager.Client`). Entity Framework migrations are in `Infrastructure/Migrations`; API controllers and configuration are in the API project. No test projects are currently present. Keep new code in the layer that owns its responsibility and avoid putting business rules in controllers.

## Build, Test, and Development Commands

Run .NET commands from the repository root:

- `dotnet build NationalTeamManager/NationalTeamManager.slnx` builds the solution.
- `dotnet run --project NationalTeamManager/NationalTeamManager.Api` starts the API.

Run client commands from `NationalTeamManager/NationalTeamManager.Client`:

- `npm install` installs dependencies.
- `npm run dev` starts the Vite development server.
- `npm run build` runs TypeScript project checks and creates a production bundle.
- `npm run lint` runs ESLint.
- `npm run preview` serves the built client locally.

There is no repository test suite yet. When adding tests, use the framework appropriate to the project and document its run command.

## Coding Style & Naming Conventions

Follow the existing C# conventions: four-space indentation, PascalCase for types and public members, and descriptive async/service names. Keep API, application, domain, and infrastructure concerns in their corresponding projects. In the client, use TypeScript/React conventions, existing ESLint configuration, and the established component and file naming in neighboring code. Run `npm run lint` and `npm run build` after client changes.

## Testing Guidelines

Add focused tests alongside new behavior as test projects are introduced. Cover API validation, application behavior, and persistence mapping at the layer where each rule belongs. Run `dotnet build` and the relevant `dotnet test` project before submitting .NET changes; run the client lint and build commands for frontend changes.

## Commit & Pull Request Guidelines

Recent history uses Conventional Commit-style subjects, for example `feat: implement team crud and search` and `fix: ...`. Use a concise `feat:`, `fix:`, `refactor:`, or similarly scoped prefix. Pull requests should summarize the change, note relevant issue context, list validation commands and outcomes, and include screenshots for visible client changes. Call out database migrations and configuration changes explicitly.

## Security & Configuration

Keep secrets and machine-specific values out of tracked settings. Use development configuration or environment variables for local credentials, and never commit API keys for external data providers.

## Working agreement

- Keep each change within the requested task. Preserve existing behavior, the user's uncommitted work and unrelated files. Check the working tree before editing.
- Introduce business rules and restrictions only when requested. Follow the established layer boundaries, names and data contracts.
- Discuss database changes with the project owner before editing the schema or its EF mapping, creating or changing migrations, or applying migrations or other database changes. Running the importer writes to SQL Server and calls RapidAPI.
- Run the relevant build and formatting checks after implementation. Report their actual outcomes and any checks that could not run.
- Obtain explicit approval before creating a commit.

## Dependency map

The solution is `NationalTeamManager/NationalTeamManager.slnx`. The five C# projects target .NET 10 with nullable reference types. The sixth project is the React client.

| Project | Direct project references | Responsibilities |
| --- | --- | --- |
| Domain | None | Entities, enums and the integer-key `IEntity` interface. |
| Application | Domain | Feature-specific query records, DTOs, service contracts, FluentValidation validators, shared paging and repository contracts. |
| Infrastructure | Application, Domain | EF Core 10 SQL Server context, entity configurations, migrations, repository, service implementations and Mapster mappings. `AddInfrastructure` registers these dependencies. |
| Api | Application, Infrastructure, Client (build integration) | HTTP controllers, validation filter, exception handling, OpenAPI and SPA hosting. The client reference has `ReferenceOutputAssembly=false`. |
| DataImporter | Application, Infrastructure | Separate console entry point. A named HTTP client reads SofaScore through RapidAPI. Importers use the shared DbContext directly. |
| Client | None | React 19, TypeScript and Vite. See the client integration notes below. |

## Domain and persistence

- A player optionally belongs to a team and has a history of market values. The player and recording timestamp identify a unique valuation. Deleting a team clears the player link. Deleting a player cascades to valuations.
- A competition has editions, and an edition has stages. A match requires an edition and two team references, one home and one away. Its stage is optional. Match incidents belong to a match and store provider player identifiers and names.
- The match-details work introduces optional venue and referee references, period scores, timing data and a processing timestamp. Review the working tree and migration history together when working on this feature.
- Entity mappings implement `IEntityTypeConfiguration<T>` in `NationalTeamManager.Infrastructure/Persistence/Configurations`. The DbContext discovers them by assembly. Imported entities generally have a unique index on external ID and data source. Incidents use data source and external key.
- Both backend entry points use `ConnectionStrings:DefaultConnection`. Importer credentials belong in user secrets or environment configuration. The provider key is `RapidApi:ApiKey`.

## Migration history

The migration chain in `NationalTeamManager.Infrastructure/Migrations` starts with `InitialCreate`, followed by `AddTeamsAndCompetitionStructure`, `AddMatchIncidents` and `AddMatchIncidentExternalKeyAndMatchStatus`. The match-details work adds `AddMatchDetails` and `AddMatchDetailsProcessingMarker`. Migration files describe the intended schema. Applied versions are recorded separately in the target database.

When reviewing upgrades from older databases, check how stored values reach the current DTO. For example, the original `Matches.Venue` contains text. The match-details migration adds a nullable `VenueId` without copying those names. The current `MatchDto.Venue` reads the related venue name, so a legacy name alone will produce a null API value.

## HTTP API

- Keep HTTP status handling in controllers, query and input contracts in Application, and EF query execution and Mapster mapping in Infrastructure. Pass cancellation tokens through asynchronous operations. Reuse the existing `PagedResult<T>` for paged searches.

All six resources support paged searches. The query records default to page 1 and 20 items. Validators accept page sizes from 1 to 100.

| Resource | Collection | Item at `/{id:int}` | Filters |
| --- | --- | --- | --- |
| `/api/players` | GET, POST | GET, PUT, DELETE | Search, position, team |
| `/api/teams` | GET, POST | GET, PUT, DELETE | Search, country, national-team flag |
| `/api/matches` | GET | GET | Date range, team, edition, stage |
| `/api/competitions` | GET | GET | Search, country |
| `/api/competition-editions` | GET | GET | Competition, search, year |
| `/api/competition-stages` | GET | GET | Edition, search |

In development, Swagger UI at `/swagger` reads `/openapi/v1.json`. The global action filter returns `ValidationProblemDetails` for invalid input. The exception handler logs unhandled exceptions and returns a generic HTTP 500 `ProblemDetails` response.

## SofaScore imports


- Put external JSON contracts in `DataImporter/Providers/SofaScore/Models`, provider calls behind `ISofaScoreClient`, and entity reconciliation in `DataImporter/Importers`. Keep provider DTOs separate from API response DTOs.
- Inspect `DataImporter/Program.cs` before a run. Importer registration and execution are separate. The current entry point imports matches for external team 4709 and then processes eligible stored SofaScore matches for details.

| Importer | Provider calls | Data affected |
| --- | --- | --- |
| `PlayerImporter` | `teams/get-squad` | Players, their teams and market valuations recorded by UTC date. |
| `MatchImporter` | `teams/get-last-matches`, `teams/get-next-matches` | Merges both lists by event ID and reconciles teams, competitions, editions, stages and matches. |
| `MatchDetailsImporter` | `matches/detail` | Updates scores, timing, status, venue, referee and `DetailsImportedAt`. Live matches and records missing selected optional details can be fetched again. |
| `MatchIncidentImporter` | `matches/get-incidents` | Processes finished matches, using a generated key when the provider omits an incident ID. |

The player and incident importers are registered but their calls in `Program.cs` are commented out. The details pass selects eligible records across stored SofaScore matches, including those belonging to other teams.

## Client integration and formatting

- The client starts in `src/main.tsx` and renders `App.tsx`. The current screen and Vite proxy use the weather forecast template. Vite proxies only `/weatherforecast`. The API exposes its football resources under `/api` and has no weather controller. Material UI, TanStack Query and Orval are declared dependencies. The repository has no domain screens or configured Orval generation command yet.
- From `NationalTeamManager/`, restore the pinned formatter with `dotnet tool restore` and check changed C# files with `dotnet csharpier check <path>`. The local tool manifest pins CSharpier 1.3.0. Limit formatting edits to files in scope.
- For client changes, run both ESLint and the production build using the commands above. Prettier is a development dependency. The package currently has no formatting script.
