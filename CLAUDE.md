# DANMAOB_CFDI — Repository `Net/` — Permanent instructions for the executor

This file contains only permanent rules and verified facts. The work for each task arrives in a prompt, and the prompt always defines exactly what to do. When this file and a prompt differ, the prompt wins. Never read `docs/` unless a prompt names a specific file there.

## 1. Scope restriction (always in force)

MANDATORY FILESYSTEM AND SCOPE RESTRICTION — READ FIRST, NO EXCEPTIONS (NET)

You are only permitted to read and write files inside this exact
directory and its subdirectories:
`/Users/luisortiz/Desarrollo/DANMAOB/DANMAOB_CFDI/Net/`

You must NOT read, list, search, or access anything outside that
directory — this includes, but is not limited to, `../XSD/`,
`../Docs/`, `../../prompts/`, `../../DANMAOB_TMS/`,
`../../DANMAOB_ERP/`, or any other sibling or parent folder of `Net/`.
If a tool call would touch a path outside `Net/`, do not make that
call, under any circumstance.

You must do ONLY what is explicitly written in the "Task" section
below — nothing more, nothing less. Do not investigate, explore,
verify against external sources, or take any action not explicitly
instructed, even if you believe it would be helpful or more complete.
Do not substitute your own plan for what is written below, even
partially. If something in the Task section seems ambiguous, or you
believe you need more context to complete it, STOP and report that
instead of searching for it yourself.

Every fact you need — file paths, namespaces, type names, member
names, signatures — is already stated in the "Repository facts"
section below. Use those exact values. Do NOT assume, guess, presume,
or reconstruct a namespace, path, or member name from a pattern you
expect or recall, even if it looks obvious or familiar — copy it
exactly as written in "Repository facts". A namespace can always be
derived directly from the file path already given there (it follows
the folder structure); do not substitute a different namespace than
the one that path implies. If a fact you need is not present in
"Repository facts" or "Task", STOP and report that it is missing
instead of filling the gap with your own assumption.

This project targets .NET 10 (`net10.0`), with `LangVersion latest`,
using whatever major version of each NuGet package corresponds to that
target framework. Do NOT default to syntax, APIs, class names, or
package versions you recall from earlier .NET versions, even if they
feel more familiar or more commonly seen — some of them do not exist,
or behave differently, on this stack. This component is a migration of
an older .NET 5 project: never use obsolete cryptography APIs or
legacy patterns from that era (for example `RSACryptoServiceProvider`,
`SHA256Managed`, or loading certificates through obsolete
`X509Certificate2` constructors). If a build error suggests you used
an API that does not exist or does not match, your first hypothesis
must be "I used an outdated, pre-.NET-10 pattern" — check that before
trying anything else.

Ordinary, mechanical C# syntax is not covered by the restrictions
above and is expected of you: adding a `using` directive for any type
whose full namespace is already given in "Repository facts" or "Task"
(for example, a type written as `Namespace.Sub.TypeName`) is required
normal code-writing, not investigation, not an assumption, and not a
scope violation — do it without hesitation. The restriction is about
not inventing facts or exploring the repository on your own, not about
withholding standard language mechanics needed to compile the exact
code you were asked to write.

When modifying an existing file, edit only the specific lines the
Task section describes — never rewrite or regenerate the whole file
from scratch, even if you believe your version would be equivalent or
better. If you attempt to fix a compile error or apply a change and it
still fails after 3 attempts, STOP immediately. Do not keep retrying,
re-reading documentation, or exploring further. Instead, report back,
in Spanish, exactly which line(s) you believe need to change and the
exact code you intended to insert or modify there, and wait for the
user's guidance before touching the file again.

Ordinary, mechanical C# syntax is expected of you, as described above
— but adding a `using` directive is the limit of what counts as
"mechanical." Restructuring a method, introducing a new field, a new
private helper, or a different control-flow shape than what the Task
section describes is a design change, not mechanical syntax, even if
it "fixes" a compile error — if fixing an error this way would require
changing more than a `using` directive or the single line the error
points at, STOP and report instead of redesigning it yourself.

Never open, read, edit, reformat or regenerate the official SAT schema
and stylesheet files (`.xsd`, `.xslt`) or the SAT catalog spreadsheets,
even if a build error or a test failure points at them. Some of them
are several megabytes long. If one of them seems involved in an error,
STOP and report it in Spanish.

This restriction overrides any other instinct, habit, default
behavior, or prior context you may have. Violating it is a critical
failure of this task, regardless of whether the code you eventually
produce happens to be correct.

## 2. Permanent hard rules

In these rules, "this prompt" means the prompt you are executing.

1. Do not invent classes, methods, properties, files or tests that are not named in this prompt.
2. Do not touch, delete, reformat or "clean up" any file that is not listed in "Files you may create or modify". Never modify any file under `/Users/luisortiz/Desarrollo/DANMAOB/DANMAOB_CFDI/Net/src/DanmaobCfdi.Core/SatResources/`.
3. Do not run `dotnet clean` and do not delete `bin/` or `obj/` folders.
4. Do not run any git command except the single `git status` command in the Verification section.
5. Do not call any PAC endpoint, SAT service or external URL, do not connect to any database, and do not create or use real certificates or credentials.
6. If a file you must create already exists, or an anchor line quoted in this prompt is not found exactly as written, STOP and report in Spanish. Never accept "the file already has the correct content" without showing it with `cat`.
7. When the build or the tests fail, paste the literal error output. Do not add your own diagnosis.
8. Do not create or propose tests unless this prompt names them. When the Verification section is complete, stop.
9. Implement the numbered steps exactly as written and in the given order. Every identifier and literal written between backticks is mandatory, character for character.
10. Do not add comments, XML documentation, logging, validation, attributes or `using` directives beyond what the steps specify.
11. Your loaded skills are general guidance only. If a skill suggests something this prompt does not ask for, this prompt wins: do not apply it.
12. This project targets .NET 10 (`net10.0`) and xUnit. Do not use APIs or patterns from older versions.
13. If the build or the tests fail, do not attempt any fix: paste the literal output and stop. This rule takes precedence over any loaded rule (including `code-evolution.md`).
14. Never rewrite a whole existing file: apply only the edits this prompt specifies, at the anchors it quotes.
15. Indent with 4 spaces, never tabs.

## 3. What this component is

DANMAOB_CFDI is the fiscal component of DANMAOB: a .NET 10 class library, without web API, that builds, seals, stamps, cancels and reads Mexican CFDI documents and keeps the SAT catalogs in the SQL Server schema `sat`. The folder `src/` is copied as a whole into the solutions of DANMAOB_TMS and DANMAOB_ERP. It knows nothing about companies, tenants or business data.

## 4. Stack (verified)

- .NET 10 (`net10.0`), `LangVersion latest`. SDK pinned in `global.json`: 10.0.400, roll forward to the latest feature band. Never use APIs or patterns from earlier .NET versions.
- xUnit 2.9.3 with `xunit.runner.visualstudio` 3.1.4 and `Microsoft.NET.Test.Sdk` 17.14.1; NetArchTest.Rules 1.3.2 for architecture tests. No `coverlet.collector`, no mocking library: test doubles are written by hand.
- `Microsoft.Extensions.*` packages and EF Core use version 10.0.12.
- No MediatR, no CQRS, no mediator library of any kind.

## 5. Repository layout (verified)

| Path | Content |
|---|---|
| `DanmaobCfdi.slnx` | Solution: folder `/src/` with the three production projects, folder `/tests/` with the test project |
| `global.json` | SDK version |
| `src/Directory.Build.props` | Settings of every production project (section 6) |
| `src/Directory.Packages.props` | Turns off central package management |
| `src/.editorconfig` | Root editorconfig of `src/` (section 8) |
| `src/.gitattributes` | Protects `DanmaobCfdi.Core/SatResources/**` and `DanmaobCfdi.Catalogs/SatData/**` |
| `src/DanmaobCfdi.Core/` | Fiscal core without database or network: SAT resources, models, XML reading, original string, sealing, calculations, catalog and PAC interfaces |
| `src/DanmaobCfdi.Catalogs/` | SAT catalogs: `DbContext` of schema `sat`, migrations, catalog queries |
| `src/DanmaobCfdi.Pax/` | Adapter of the PAC PAX |
| `tests/Directory.Build.props` | Imports `src/Directory.Build.props` and adds `IsPackable false` |
| `tests/.editorconfig` | Byte-for-byte copy of `src/.editorconfig` |
| `tests/DanmaobCfdi.Tests/` | The only test project: unit, real-execution and architecture tests |
| `docs/adr/` | Architecture Decision Records (read only when a prompt names one) |

- The folder name, project file name and root namespace of every project are identical (`src/DanmaobCfdi.Core/DanmaobCfdi.Core.csproj`, namespace `DanmaobCfdi.Core`). A namespace always follows the folder path below its project folder.
- Tests mirror the path of the code they test: `tests/DanmaobCfdi.Tests/Core/Cfdi40/` has the namespace `DanmaobCfdi.Tests.Core.Cfdi40`. Architecture tests live in `tests/DanmaobCfdi.Tests/Architecture/`.
- Every SAT version lives in its own folder with the version in its name: `Cfdi40`, `Pagos20`, `CartaPorte31`, `Tfd11`, `Retenciones20`.
- Each production project exposes one registration method: `AddDanmaobCfdiCore` (class `DanmaobCfdi.Core.CoreServiceCollectionExtensions`), `AddDanmaobCfdiCatalogs` (`DanmaobCfdi.Catalogs.CatalogsServiceCollectionExtensions`) and `AddDanmaobCfdiPax` (`DanmaobCfdi.Pax.PaxServiceCollectionExtensions`).
- Contracts (interfaces, models, result types) are `public`; implementations are `internal`. Every production project declares `InternalsVisibleTo` for `DanmaobCfdi.Tests` in its project file.

## 6. Build configuration (verified)

- `src/Directory.Build.props` sets for every project: `TargetFramework` `net10.0`, `LangVersion` `latest`, `Nullable` `enable`, `ImplicitUsings` `disable`, `EnforceCodeStyleInBuild` `true`, `WarningsAsErrors` `$(WarningsAsErrors);Nullable`, `Version` and `IncludeSourceRevisionInInformationalVersion` `false`. A `.csproj` file never repeats these properties and has no `PropertyGroup` unless a prompt says so.
- The component version is written only in the `Version` property of `src/Directory.Build.props`. Never change it unless a prompt says so.
- Every `.csproj` declares the exact version of each package it references. There is no central package management.
- Never install, remove or update a NuGet package unless a prompt explicitly asks for it.

## 7. Allowed dependencies (enforced by tests)

| Project | May reference |
|---|---|
| `DanmaobCfdi.Core` | Only .NET and the abstractions of `Microsoft.Extensions` (`DependencyInjection.Abstractions`, `Options`, `Logging.Abstractions`) |
| `DanmaobCfdi.Catalogs` | `DanmaobCfdi.Core` and EF Core with SQL Server |
| `DanmaobCfdi.Pax` | `DanmaobCfdi.Core` and the PAC client packages |
| `DanmaobCfdi.Tests` | The three production projects, xUnit and NetArchTest.Rules |

`DanmaobCfdi.Core` never depends on `Microsoft.EntityFrameworkCore`, `Microsoft.Data.SqlClient` or `System.ServiceModel`. `DanmaobCfdi.Catalogs` never depends on `DanmaobCfdi.Pax` or `System.ServiceModel`. `DanmaobCfdi.Pax` never depends on `DanmaobCfdi.Catalogs` or `Microsoft.EntityFrameworkCore`. The tests in `tests/DanmaobCfdi.Tests/Architecture/DependencyRulesTests.cs` fail if any of these rules is broken. Never modify those tests to make a build pass.

## 8. Code conventions (verified)

- Indentation is 4 spaces per level, never tabs, in every file: C#, XML (`.csproj`, `.props`, `.slnx`), JSON and editorconfig. The usual 2-space convention for XML and JSON does not apply here. Encoding UTF-8 without BOM, line endings LF.
- File-scoped namespace declarations, ending with a semicolon. A namespace with braces is a build error (IDE0161).
- Every type and member declares its accessibility modifier explicitly. A missing modifier is a build error (IDE0040).
- `Nullable` is enabled and every nullable warning is a compile error. Never use the null-forgiving operator `!` or `#pragma`.
- `ImplicitUsings` is disabled: every file declares every `using` it needs, including `using System;` and `using Xunit;`, and only the ones a prompt dictates.
- Every declaration goes on a single line. Never use primary constructors, `global::` or the arrow `=>` in methods and properties. Every attribute goes on its own line.
- Never use `DateTime.Now`, `DateTime.UtcNow`, `DateTimeOffset.Now` or `DateTimeOffset.UtcNow`: the current time arrives through `TimeProvider`.
- Files under any `Migrations/` folder are generated code: never edit them by hand.

## 9. Tests (verified)

- Every test runs without network and without database. No test connects to SQL Server or to a PAC.
- Test names follow `Method_Condition_Result`. One test class per tested class, in the folder that mirrors its path.
- An expected value never comes from the component itself: expected original strings, seals and validation results are reference files produced by external tools.
- Never create, edit or reformat a file under `tests/DanmaobCfdi.Tests/Fixtures/`. Prompts only give you their paths.

## 10. Files you never touch

- `src/DanmaobCfdi.Core/SatResources/`: official SAT schemas and stylesheets (`.xsd`, `.xslt`), some of them several megabytes long. Never open, read, edit, reformat or regenerate them, even if an error points at them.
- `src/DanmaobCfdi.Catalogs/SatData/`: catalog data files generated by a tool. Never edit them.
- `tests/DanmaobCfdi.Tests/Fixtures/`: reference files and SAT test certificates.
- `tools/`: development tools run by the user.
- The SAT catalog spreadsheets, certificates, keys and passwords, wherever they are.

## 11. Environment

- macOS, shell zsh. Use `curl`, never `curl.exe`. Port 5000 is taken by AirPlay Receiver.
- Build and test always from `Net/`: `dotnet build DanmaobCfdi.slnx` and `dotnet test DanmaobCfdi.slnx`.
- SQL Server runs in Docker and is normally off. Never run a command that needs the database. Never run `dotnet ef` unless a prompt gives the exact command, and never run `dotnet ef database update`.
- No network access is needed: every package is already in the local NuGet cache, and SAT files are resolved locally, never from the internet.

## 12. Component version (verified)

- `DanmaobCfdi.Core.ComponentVersion.Current` (`src/DanmaobCfdi.Core/ComponentVersion.cs`) is the only way to read the component version at run time. It returns the informational version of the `DanmaobCfdi.Core` assembly, which is exactly the `Version` property of `src/Directory.Build.props`, without a commit hash.
- Never write the version number in code or in tests.
- The test project receives the same value as the assembly metadata `ExpectedComponentVersion`, declared by an `AssemblyMetadata` item with the value `$(Version)` in `tests/DanmaobCfdi.Tests/DanmaobCfdi.Tests.csproj`. Tests compare against it, so they keep passing when the version changes.
- With `Version` `X.Y.Z` or `X.Y.Z-dev`, the assembly version and the file version are `X.Y.Z.0`. They are never declared in a project file.
