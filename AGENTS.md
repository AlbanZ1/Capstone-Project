# Repository Guidelines

## Project Structure & Module Organization

This repository contains one .NET 8 ASP.NET Core MVC solution. `Auctions.sln` is the solution entry point, and the main web project lives in `Auctions/`.

- `Auctions/Controllers/`: MVC controllers for request handling.
- `Auctions/Models/`: domain and view model classes.
- `Auctions/Data/`: EF Core database context, migrations, and service classes.
- `Auctions/Views/`: Razor views grouped by controller, with shared layouts in `Views/Shared/`.
- `Auctions/Areas/Identity/`: ASP.NET Identity UI pages.
- `Auctions/wwwroot/`: static CSS, JavaScript, Bootstrap/jQuery libraries, and images.
- `Auctions/Program.cs`: startup, dependency injection, routing, Identity, database, and Azure wiring.

Do not edit or commit `bin/` and `obj/` outputs.

## Build, Test, and Development Commands

- `dotnet restore Auctions.sln`: restore NuGet packages.
- `dotnet build Auctions.sln`: compile the project and validate references.
- `dotnet run --project Auctions/Auctions.csproj`: start the MVC app locally.
- `dotnet ef database update --project Auctions/Auctions.csproj`: apply EF Core migrations to the configured SQL Server database.
- `dotnet test Auctions.sln`: run tests when a test project is added.

Run commands from the repository root. Full local workflows require SQL Server and Azure Blob Storage configuration.

## Coding Style & Naming Conventions

Use standard C# conventions: four-space indentation, PascalCase for public types and members, camelCase for locals and parameters, and async methods ending in `Async`. Keep nullable reference types enabled.

Follow the MVC naming pattern: controllers end with `Controller`, Razor views match action names, and EF migrations stay in `Auctions/Data/Migrations/`. Keep business logic out of Razor views; prefer controllers or services under `Data/Services/`.

## Testing Guidelines

No test project is currently present. For new tests, add `Auctions.Tests` and include it in `Auctions.sln`. Prefer xUnit or MSTest with behavior-focused names, for example `CreateListing_ReturnsRedirect_WhenModelIsValid`.

Before opening a PR, at minimum run `dotnet build Auctions.sln`. Run `dotnet test Auctions.sln` once tests exist.

## Commit & Pull Request Guidelines

Recent history uses short, imperative commit messages such as `Clean up exposed configuration secrets`. Keep commits focused.

Pull requests should include a concise summary, test/build results, linked issues if applicable, and screenshots for UI changes. Call out database migrations, configuration changes, or Azure resource requirements explicitly.

Do not commit or push to GitHub with any co-authoring features. I will do all of that myself in order to have the work seem like it's all built by me.

## Security & Configuration Tips

Do not commit real connection strings, Azure storage keys, passwords, or user secrets. Use `appsettings.Development.json`, environment variables, or .NET user secrets for local values. Review `appsettings.json` carefully because this app integrates with SQL Server, Azure Blob Storage, and Azure configuration services.
