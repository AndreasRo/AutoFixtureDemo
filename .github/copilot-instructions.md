<!-- Auto-generated: tailored guidance for AI coding agents working on this repository -->
# Copilot instructions for AutoFixtureDemo

Short, practical guidance for an AI coding agent making edits in this repository.

- Project type: small ASP.NET Core Web API (minimal Program.cs) with a lightweight in-memory "repository" used for demo/testing. See `Program.cs` for service wiring.
- Purpose: demo of domain objects, mapping (AutoMapper), and AutoFixture unit test customizations. Main areas to touch: `Controllers/`, `Services/`, `Database/`, `Mapping/`, `DomainObjects/`, and `AutoFixtureDemo.UniTests/`.

Key patterns and files
- Program wiring: `Program.cs` registers services as singletons: `IWeatherForecastService -> WeatherForecastService` and `ILocationRepository -> LocationRepository` and configures AutoMapper with `MappingProfile`.
- Controller layer: `Controllers/WeatherForecastController.cs` expects a query parameter `location` and returns `LocationDto` mapped from the domain `Location`. Keep validation semantics (BadRequest when missing) intact.
- Service layer: `Services/WeatherForecastService.cs` is a thin adapter that fetches a `LocationEntity` from `ILocationRepository` and maps it to the domain `Location` using `IMapper`.
- Repository: `Database/LocationRepository.cs` stores an in-memory list of `LocationEntity` and returns a dummy location when not found (see `GetDummy`). Be conservative when changing repository behavior — tests and demos rely on deterministic dummy generation.
- Mapping: `Mapping/MappingProfile.cs` contains AutoMapper mappings between Entities -> DomainObjects -> DTOs. Temperature wrappers (Celsius/Fahrenheit) and `Text` wrappers mean mappings often construct small value objects (e.g., new Celsius(src.TemperatureC)). Preserve those conversions.
- Domain types: `DomainObjects/` contains small value objects (`Text`, `Celsius`, `Fahrenheit`) and aggregate (`Location`, `WeatherForecast`). These wrap primitives and perform validation. Use their constructors when creating domain objects.
- Tests/customization: `AutoFixtureDemo.UniTests/AutoFixtureCustomizations/DefaultTestCustomization.cs` customizes AutoFixture to avoid recursion and to create valid `Text`, `DateOnly`, and `Celsius` values. Unit tests rely on these fixtures; changing them will affect many tests.

Developer workflows (how to build, run, test)
- Build & run (dotnet CLI): run the app from repository root

    dotnet build
    dotnet run --project AutoFixtureDemo/AutoFixtureDemo.csproj

- Run unit tests (project contains xUnit/NUnit style tests via the `AutoFixtureDemo.UniTests` project):

    dotnet test AutoFixtureDemo.UniTests/AutoFixtureDemo.UnitTests.csproj

- Swagger UI is enabled in Development per `Program.cs` so run the app and visit `/swagger` to explore endpoints.

Project-specific conventions and gotchas
- Value-object wrappers: many simple types (city/country/temperature) are wrapped in small domain classes that validate inputs. Prefer using those constructors rather than bypassing validation.
- AutoMapper is used widely to convert between Entities, DomainObjects, and DTOs. When adding fields, update `Mapping/MappingProfile.cs` accordingly and ensure tests cover mapping.
- In-memory repository uses collection initializer syntax and the `GetDummy` helper which uses C# array/range syntax; be careful when editing to preserve the demo behavior.
- Tests rely on AutoFixture customization in `AutoFixtureDemo.UniTests/AutoFixtureCustomizations/DefaultTestCustomization.cs` to produce valid values and avoid recursion. If adding new domain types, extend this customization accordingly.

Integration points & dependencies
- AutoMapper (configured in Program.cs). Update mappings when domain/dto/entity shapes change.
- AutoFixture (tests). See `AutoFixtureDemo.UniTests` for example customizations.
- No external DB or network: repository is in-memory. Safe to run locally without additional infrastructure.

Editing guidance & examples
- When adding a new field to `WeatherForecastEntity`:
  - Update `Database/Entities/WeatherForecastEntity.cs` (if present),
  - Add mapping in `Mapping/MappingProfile.cs` for Entity->Domain and Domain->DTO conversions,
  - Update DTO under `Controllers/DTO/` and adjust controller mapping usage,
  - Add or update AutoFixture customization to generate valid values for new types.

- Example: to set TemperatureC as an int in DTO -> mapping uses `.ForMember(dest => dest.TemperatureC, opt => opt.MapFrom(src => src.TemperatureC.Value))` (see `MappingProfile`). Mirror this for any added value-wrapping types.

Safety and tests
- Run `dotnet test` after changes. Be particularly mindful of test fixture behavior; unit tests expect the AutoFixture customization to produce valid DateOnly and Text values.

If something is unclear
- Ask for which area you want to change (controller, service, mapping, tests) and I'll point to the exact files and tests to run. If adding new public APIs, add mapping tests and update AutoFixture customizations.

-- End of copilot instructions
