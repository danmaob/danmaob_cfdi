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
| `DanmaobCfdi.Catalogs` | `DanmaobCfdi.Core`, EF Core with SQL Server and the pinned `System.Security.Cryptography.Pkcs` (section 16) |
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

## 13. SAT resources (verified)

- `src/DanmaobCfdi.Core/SatResources/` holds the 60 official SAT files (`.xsd` and `.xslt`) downloaded on 2026-10-16, byte for byte identical to the published ones. Many of them have CRLF line endings or a BOM: that is how the SAT publishes them, and `src/.gitattributes` keeps them unchanged.
- Each file lives at the path of its official address without the scheme and the domain: `http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd` is `SatResources/sitio_internet/cfd/4/cfdv40.xsd`. Every address that a file includes or imports has its local file.
- Entry files (no other file includes them): `sitio_internet/cfd/4/cadenaoriginal_4_0/cadenaoriginal_4_0.xslt` (includes 33 stylesheets), `sitio_internet/cfd/4/cfdv40.xsd`, `sitio_internet/cfd/Pagos/Pagos20.xsd`, `sitio_internet/cfd/CartaPorte/CartaPorte31.xsd`, `sitio_internet/cfd/implocal/implocal.xsd`, `sitio_internet/cfd/leyendasFiscales/leyendasFisc.xsd`, `sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd`, `esquemas/retencionpago/2/retencionpagov2.xsd` and `esquemas/retencionpago/2/retenciones.xslt` (includes 13 stylesheets).
- `SatResources/SHA256SUMS`: the first line is `# juego SAT 2026-10-16`; every other line has the format of `shasum -a 256`, with the path relative to `SatResources/`. The user's copy script generates it. Never edit it.
- Never open, list the content of, or modify these files. A prompt gives you their paths and, when needed, the facts about them.

## 14. SAT resource resolution and compilation (verified)

- `src/DanmaobCfdi.Core/DanmaobCfdi.Core.csproj` embeds every `.xsd` and `.xslt` file of `SatResources/` with the logical name equal to its path relative to `SatResources/`, with forward slashes (`sitio_internet/cfd/4/cfdv40.xsd`). `SHA256SUMS` is not embedded. Never change that item.
- `DanmaobCfdi.Core.Resources.SatResourceResolver` (`internal sealed`, derives from `System.Xml.XmlResolver`) is the only resolver used for SAT schemas and stylesheets. It turns `http://www.sat.gob.mx/<path>` or `https://www.sat.gob.mx/<path>` into the embedded resource `<path>` and throws `XmlException` for any other address. It never uses the network or the disk. Never use `XmlUrlResolver` or any other resolver.
- `DanmaobCfdi.Core.Resources.SatCompiledResources` (`internal sealed`, registered as a singleton by `AddDanmaobCfdiCore` together with `SatResourceResolver`) compiles each SAT artifact once, the first time it is requested, with `Lazy<T>` and `LazyThreadSafetyMode.ExecutionAndPublication`, and returns the same instance afterwards: `CfdiOriginalStringTransform` and `RetencionesOriginalStringTransform` (`XslCompiledTransform`, loaded with `XsltSettings.Default`: no scripts, no `document()`, no debugging) and `CfdiSchemas` and `RetencionesSchemas` (`XmlSchemaSet`, compiled). Never compile a SAT stylesheet or schema anywhere else.
- SAT files are read with `DtdProcessing.Prohibit`. Third-party XML documents never use the SAT resolver.
- The test project copies `SatResources/SHA256SUMS` to its output folder as `SatResources/SHA256SUMS` (`AppContext.BaseDirectory`). A test compares every embedded resource with it.
- Tests that need the compiled SAT artifacts share one `private static readonly` instance of `SatCompiledResources` per test class: compiling the SAT schemas takes seconds.

## 15. Reference files and SAT test certificates (verified)

- `tests/DanmaobCfdi.Tests/Fixtures/Csd/` holds the public SAT test certificates: `EKU9003173C9` (legal entity, certificate number `30001000000500003416`) and `WERX631016S30` (individual, certificate number `30001000000500003450`), as `<RFC>.cer`, `<RFC>.key` and `<RFC>.password.txt`. `.gitignore` excludes `.cer` and `.key` everywhere except in that folder. No other certificate, key or password ever enters the repository.
- `tests/DanmaobCfdi.Tests/Fixtures/Cfdi40/ingreso-basico.xml` is a sealed CFDI 4.0 from `EKU9003173C9` to `WERX631016S30`; `ingreso-basico.cadena.txt` is its original string produced by `xsltproc`, and `ingreso-basico.sello.txt` its seal produced by OpenSSL. Both are UTF-8 without BOM and without a final line break.
- `tests/.gitattributes` marks `DanmaobCfdi.Tests/Fixtures/**` as `-text`. The test project copies every file under `Fixtures/` to its output folder (`AppContext.BaseDirectory`), keeping the folders.
- Reference files are generated by the user with `tools/fixtures/generar-referencia.sh` from the templates in `tools/fixtures/plantillas/`, never by the component. Never create, edit or regenerate a file under `Fixtures/` or `tools/`.
- A test that compares with a reference file reads the expected value from that file. If the values differ, the test fails: never adjust the reference file or the test to make them match.

## 16. SAT catalog persistence (verified)

- `DanmaobCfdi.Catalogs` references `Microsoft.EntityFrameworkCore.SqlServer` 10.0.12, `Microsoft.EntityFrameworkCore.Design` 10.0.12 (`PrivateAssets="all"`) and `System.Security.Cryptography.Pkcs` 10.0.13. The last one is a direct pin: EF Core brings version 9.0.11 transitively, and every version up to 10.0.12 has known vulnerabilities (GHSA-846f-f764-76mx, GHSA-2fmv-rv2c-59x7); 10.0.13 is the first patched version of the 10.0 line. Never remove it or change its version unless a prompt says so. The global tool `dotnet ef` is version 10.0.12.
- `src/DanmaobCfdi.Catalogs/Persistence/` holds the persistence of the SAT catalogs, namespace `DanmaobCfdi.Catalogs.Persistence`, every type `internal`:
    - `SatDbContext` (`internal const string Schema = "sat"`): default schema `sat`, no `DbSet` properties (queries use `Set<T>()`), and `OnModelCreating` applies every `IEntityTypeConfiguration` of the assembly with `ApplyConfigurationsFromAssembly`. A new table is added with its entity and its configuration class; `SatDbContext` is never modified for that.
    - `SatDbContextOptions.Configure(DbContextOptionsBuilder builder, string connectionString)`: SQL Server with the migration history in `sat.__EFMigrationsHistory`. It is the only place where SQL Server is configured.
    - `SatDbContextFactory`: design-time factory for `dotnet ef`. It reads the connection string only from the environment variable `DANMAOB_CFDI_SAT_CONNECTION`, set in the same command that runs `dotnet ef`, never exported.
    - `CatalogoVersion` and `CatalogoVersionConfiguration`: table `sat.CatalogoVersion`, key `Catalogo`.
- `AddDanmaobCfdiCatalogs(this IServiceCollection services, string connectionString)` throws `ArgumentException` for a blank connection string and registers `SatDbContext` with `AddDbContext` (scoped). The component never reads configuration by itself.
- Migrations live in `src/DanmaobCfdi.Catalogs/Persistence/Migrations/` (generated code; never edited by hand). They are generated with: `DANMAOB_CFDI_SAT_CONNECTION="Server=localhost;Database=DanmaobCfdi;TrustServerCertificate=True" dotnet ef migrations add <Name> --project src/DanmaobCfdi.Catalogs/DanmaobCfdi.Catalogs.csproj --startup-project src/DanmaobCfdi.Catalogs/DanmaobCfdi.Catalogs.csproj --output-dir Persistence/Migrations`, from `Net/`, only when a prompt says so. `migrations add` never connects to the database. `dotnet ef database update` is run only by the user, with Docker on.
- The local development database of the component is `DanmaobCfdi`, in the user's SQL Server container.
- Tests never connect to the database: they build the model with `UseSqlServer` and a connection string without credentials, and read it with `GetRelationalModel()`.

## 17. Secure loading of third-party XML (verified)

- `src/DanmaobCfdi.Core/Reading/` holds the reading of third-party XML (ADR-05), namespace `DanmaobCfdi.Core.Reading`. What depends on a SAT version goes in the subfolders `Cfdi40`, `Pagos20`, `CartaPorte31`, `Tfd11` and `Cancelacion`; what does not depend on a version stays in `Reading/` itself.
- `RejectionReason` (`public enum`): `TooLarge = 1`, `DtdOrEntity = 2`, `TooDeep = 3`, `InvalidEncoding = 4`, `Malformed = 5`. Its values never change: it will be part of the public reading result.
- `SecureXmlLoadResult` (`internal sealed`): properties `Document` (`XDocument?`), `Reason` (`RejectionReason?`) and `IsLoaded` (`bool`, true when `Document` is not null). It is created only through `SecureXmlLoadResult.Loaded(XDocument document)` and `SecureXmlLoadResult.Rejected(RejectionReason reason)`; its constructor is private.
- `SecureXmlLoader` (`internal sealed`, without state, not registered in dependency injection yet) has `internal const int MaxSizeInBytes = 10485760`, `internal const int MaxDepth = 64` and `public SecureXmlLoadResult Load(byte[] content)`. `Load` applies, in this order: more than `MaxSizeInBytes` bytes gives `TooLarge`; bytes that are not valid UTF-8 (`System.Text.Unicode.Utf8.IsValid`) give `InvalidEncoding`; a safety read node by node with `XmlReader`, without building a tree, where an XML declaration with an `encoding` other than `UTF-8` (case-insensitive) gives `InvalidEncoding` and an element at `XmlReader.Depth` 64 or more (the root is depth 0) gives `TooDeep`; finally the `XDocument` is loaded with `LoadOptions.SetLineInfo`. An `XmlException` gives `DtdOrEntity` when the bytes contain `<!DOCTYPE`, and `Malformed` otherwise.
- Every `XmlReader` of `SecureXmlLoader` uses `DtdProcessing.Prohibit`, `XmlResolver` null and `CloseInput` true, over a read-only `MemoryStream`.
- Third-party XML is loaded only through `SecureXmlLoader`. Never call `XDocument.Load`, `XDocument.Parse`, `XmlDocument` or `XmlReader.Create` on third-party content anywhere else, and never use `SatResourceResolver` or any other resolver for it.
- A defective document never throws: `XmlException` is caught only inside `SecureXmlLoader`, and every defect becomes a `RejectionReason`.
- `tests/DanmaobCfdi.Tests/Fixtures/Invalidos/` holds synthetic negative files, copied by the user's script and never edited: `con-dtd.xml` and `entidad-externa.xml` (`DtdOrEntity`), `mal-formado.xml` (`Malformed`), `declara-iso-8859-1.xml`, `utf8-invalido.xml` and `utf16.xml` (`InvalidEncoding`).
- `tests/DanmaobCfdi.Tests/Core/Reading/SecureXmlLoaderTests.cs` builds the oversized and deeply nested documents in memory; no large file enters `Fixtures/`.

## 18. Structural validation of third-party XML (verified)

- `StructuralError` (`public sealed`, `src/DanmaobCfdi.Core/Reading/`): properties `LineNumber` (`int`), `LinePosition` (`int`) and `Message` (`string`); its constructor is `internal`. `LineNumber` and `LinePosition` are 0 when the node has no line information.
- `XmlStructuralValidator` (`internal sealed`, not registered in dependency injection yet) receives `SatCompiledResources` in its constructor, has `internal const int MaxErrors = 100` and `public IReadOnlyList<StructuralError> Validate(XDocument document)`. It validates against `SatCompiledResources.CfdiSchemas` only; the caller (detection, TS-02-3) guarantees that the document is a CFDI 4.0. An empty list means valid.
- Only `XmlSeverityType.Error` events count; warnings are ignored. At most `MaxErrors` errors are collected. Line and position come from the `IXmlLineInfo` of the node that raised the event.
- The `xs:any` wildcards of `cfd/4/cfdv40.xsd` (`ComplementoConcepto`, `Complemento`, `Addenda`) are strict. `XmlStructuralValidator` ignores every error raised inside an element that is a direct child of `cfdi:ComplementoConcepto`, `cfdi:Complemento` or `cfdi:Addenda` and whose namespace is not in the schema set (ADR-05, section 2). Never relax this further and never modify an official XSD to change it.
- `StructuralError.Message` is the validator message as is and may contain document values: never write it to a log.
- Fixtures of this area: `Fixtures/Cfdi40/con-extensiones-desconocidas.xml` (a Comercio Exterior complement and an addenda without schema; valid), and in `Fixtures/Invalidos/`: `atributo-fuera-del-xsd.xml` (one error, line 3), `varios-errores.xml` (two errors, lines 4 and 6) and `tfd-atributo-fuera-del-xsd.xml` (one error inside the Timbre Fiscal Digital, line 20). The user's script checks them with `xmllint` against every CFDI schema before copying them.
- `tests/DanmaobCfdi.Tests/Core/Reading/XmlStructuralValidatorTests.cs` shares one static `XmlStructuralValidator`, because compiling the schemas takes seconds.
- Never use `var`: every local variable is declared with its exact type.

## 19. Detection of type and version (verified)

- Public enums in `src/DanmaobCfdi.Core/Reading/` (they will be part of the public reading result; their values never change): `DocumentKind` (`Cfdi40 = 1`, `CancellationAcknowledgment = 2`) and `UnsupportedReason` (`CfdiVersion = 1`, `ComplementVersion = 2`, `Retenciones = 3`). Internal enum `DetectionOutcome` (`Supported = 1`, `Unsupported = 2`, `Unrecognized = 3`).
- `UnreadComplement` (`public sealed`, `internal` constructor): `NamespaceName` and `LocalName` (`string`) of a complement that the reading does not extract.
- `DetectionResult` (`internal sealed`, private constructor): `Outcome`, `Kind` (`DocumentKind?`), `Reason` (`UnsupportedReason?`), `DetectedVersion` (`string?`) and `UnreadComplements` (`IReadOnlyList<UnreadComplement>`, never null). Created only through `Supported(DocumentKind kind, IReadOnlyList<UnreadComplement> unreadComplements)`, `Unsupported(UnsupportedReason reason, string detectedVersion)` and `Unrecognized()`.
- `DocumentDetector` (`internal sealed`, without state, not registered in dependency injection yet) has `public DetectionResult Detect(XDocument document)`. Rules, in this order (ADR-05, section 3):
    - Root `Comprobante`: supported as `Cfdi40` only in `http://www.sat.gob.mx/cfd/4` with version `4.0`; otherwise `Unsupported` with `CfdiVersion` and the detected version. The version is read from `Version`, or from `version` in lowercase when `Version` is missing (CFDI 3.2 and Timbre Fiscal Digital 1.0), or is empty.
    - Children of `cfdi:Complemento` named `CartaPorte`, `Pagos` or `TimbreFiscalDigital` must be in `http://www.sat.gob.mx/CartaPorte31` with `3.1`, `http://www.sat.gob.mx/Pagos20` with `2.0` and `http://www.sat.gob.mx/TimbreFiscalDigital` with `1.1`; otherwise the whole document is `Unsupported` with `ComplementVersion` and the detected version written as local name, a space and version (`CartaPorte 3.0`). The rule uses the local name, never a list of old namespaces.
    - Other children of `cfdi:Complemento` whose namespace is not Carta Porte 3.1, Pagos 2.0, Timbre Fiscal Digital, `http://www.sat.gob.mx/implocal` or `http://www.sat.gob.mx/leyendasFiscales` are listed in `UnreadComplements`. `cfdi:Addenda` is never inspected.
    - Root `Retenciones` in any namespace: `Unsupported` with `Retenciones`. Root `Cancelacion` in `http://cancelacfd.sat.gob.mx`: supported as `CancellationAcknowledgment` (its verification arrives with TS-02-9). Anything else: `Unrecognized`.
- Detection runs before validation and never validates: a document that is not supported is never validated.
- Fixtures of this area are synthetic and minimal, copied by the user's script and never edited. They are NOT valid against the XSD on purpose: `Fixtures/Deteccion/` (`cfdi40-cartaporte31.xml`, `acuse-cancelacion.xml`, `cancelacion-otro-namespace.xml`, `no-reconocido.xml`) and `Fixtures/NoSoportados/` (`cfdi33.xml`, `cfdi32.xml`, `cartaporte30.xml`, `pagos10.xml`, `tfd10.xml`, `retenciones20.xml`). Never use them in validation or extraction tests.

## 20. Reading model and extraction of CFDI 4.0 (verified)

- `src/DanmaobCfdi.Core/Reading/XmlAttributeReader.cs` (`internal static`): `Required` (missing gives `string.Empty`), `Optional` (missing gives `null`), `RequiredDecimal`, `OptionalDecimal` (`decimal.Parse` with `NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign` and `CultureInfo.InvariantCulture`) and `RequiredDateTime` (`DateTime.ParseExact` with `yyyy-MM-dd'T'HH:mm:ss`, `DateTimeStyles.None`, so `DateTimeKind.Unspecified`). It only reads documents already validated against the XSD. Every reader of a SAT version uses it; never parse attribute values another way.
- The reading model of CFDI 4.0 lives in `src/DanmaobCfdi.Core/Reading/Cfdi40/`, namespace `DanmaobCfdi.Core.Reading.Cfdi40`: `ComprobanteData`, `CfdiRelacionadosData`, `EmisorData`, `ReceptorData`, `ConceptoData`, `ImpuestosData`, `TrasladoData` and `RetencionData`. They are `public sealed` classes without constructors or methods; every property is `{ get; internal init; }` and is set only in object initializers inside this component. Property names are the exact SAT attribute and element names, in Spanish. Non-nullable `string` properties are initialized with `string.Empty`, lists with `Array.Empty<T>()` and the required child objects (`Emisor`, `Receptor`) with a new empty instance; optional values are nullable and absent means `null`. The `Data` suffix avoids clashing with the emission model of `DanmaobCfdi.Core.Cfdi40` (ADR-09).
- Outside the model on purpose: `InformacionGlobal`, `ACuentaTerceros`, `InformacionAduanera`, `CuentaPredial`, `Parte` and `ComplementoConcepto`. Adding them later is a compatible change.
- `Cfdi40ImpuestosReader` (`internal static`): `ReadTraslados(XElement? impuestos)`, `ReadRetenciones(XElement? impuestos)` (both return an empty list for `null`) and `ReadImpuestos(XElement comprobante)` (returns `null` when the document has no `Impuestos`). The same `TrasladoData` and `RetencionData` serve concept-level and document-level taxes.
- `Cfdi40Extractor` (`internal sealed`, without state, not registered in dependency injection yet) has `public ComprobanteData Extract(XDocument document)`. It receives only documents already detected as CFDI 4.0 and validated; its `ArgumentException`s are for programming errors and never carry document values.
- Real fixtures, sealed with the test CSD of `EKU9003173C9` by `tools/fixtures/generar-referencia.sh` from the templates in `tools/fixtures/plantillas/Cfdi40/`: `Fixtures/Cfdi40/ingreso-flete.xml` (two concepts, discount, transferred VAT 16% and withheld VAT 4%, PPD) and `Fixtures/Cfdi40/egreso-relacionado.xml` (credit note with two groups of related CFDI), each with its `.cadena.txt` and `.sello.txt`. To change one, edit its template and run the generator again; never edit the generated files.
- `tests/DanmaobCfdi.Tests/Core/Reading/Cfdi40/Cfdi40ExtractorTests.cs` loads each fixture through `SecureXmlLoader` and extracts it.

## 21. Public API of the third-party XML reading (verified)

- The only entry point is the public interface `IThirdPartyXmlReader` (`src/DanmaobCfdi.Core/Reading/`) with `public XmlReadResult Read(byte[] content)`. It is synchronous and never throws for a document: every outcome is a result. Only a `null` argument throws (`ArgumentNullException`). The implementation `ThirdPartyXmlReader` is `internal sealed` and receives `SecureXmlLoader`, `DocumentDetector`, `XmlStructuralValidator` and `Cfdi40Extractor` in its constructor.
- `XmlReadStatus` (public enum, values never change): `Read = 1`, `Unsupported = 2`, `Unrecognized = 3`, `Invalid = 4`, `Rejected = 5`.
- `XmlReadResult` (`public sealed`, every property `{ get; internal init; }`): `Status`; `Kind` (`DocumentKind?`, set in `Read` and `Invalid`); `Cfdi` (`ComprobanteData?`, set only in `Read` of a CFDI 4.0); `UnreadComplements` (set in `Read`); `UnsupportedReason` and `DetectedVersion` (set in `Unsupported`); `StructuralErrors` (set in `Invalid`); `RejectionReason` (set in `Rejected`). Lists are never null (empty arrays by default). Later stories (Timbre Fiscal Digital, Pagos 2.0, Carta Porte 3.1, cancellation acknowledgment) only add new nullable properties; existing properties and states never change.
- `Read` applies, in this order: `SecureXmlLoader` (a rejection gives `Rejected`), `DocumentDetector` (`Unrecognized` or `Unsupported`), and only for `DocumentKind.Cfdi40`: `XmlStructuralValidator` (any error gives `Invalid`) and `Cfdi40Extractor` (`Read`). A cancellation acknowledgment gives `Read` with its `Kind`, without model and without validation, until TS-02-9 adds both.
- `AddDanmaobCfdiCore` registers as singletons: `SatResourceResolver`, `SatCompiledResources`, `SecureXmlLoader`, `DocumentDetector`, `XmlStructuralValidator`, `Cfdi40Extractor` and `IThirdPartyXmlReader` (implemented by `ThirdPartyXmlReader`). None of them keeps state between calls. A new reader component is registered there, after the existing lines.
- `tests/DanmaobCfdi.Tests/Core/Reading/ThirdPartyXmlReaderTests.cs` obtains the reader from a `ServiceCollection` with `AddDanmaobCfdiCore`, so it also tests the registration.

## 22. Timbre Fiscal Digital 1.1 (verified)

- `src/DanmaobCfdi.Core/Reading/Tfd11/`, namespace `DanmaobCfdi.Core.Reading.Tfd11`: `TimbreFiscalDigitalData` (`public sealed`, every property `{ get; internal init; }`: `Version`, `UUID`, `FechaTimbrado` (`DateTime` without time zone), `RfcProvCertif`, `Leyenda` (`string?`), `SelloCFD`, `NoCertificadoSAT` and `SelloSAT`) and `Tfd11Extractor` (`internal static`, `internal static TimbreFiscalDigitalData? Extract(XElement comprobante)`), which reads the `tfd:TimbreFiscalDigital` child of `cfdi:Complemento` and returns `null` when there is none.
- `ComprobanteData.TimbreFiscalDigital` (`TimbreFiscalDigitalData?`) is the last property of `ComprobanteData`, and `Cfdi40Extractor` fills it with `Tfd11Extractor.Extract(root)`. An unstamped CFDI 4.0 is still `Read`, with `TimbreFiscalDigital` null: whether to accept it is the product's decision. The stamp's seals are never verified (D-30).
- `tools/fixtures/generar-referencia.sh` validates with `xmllint` against `tools/fixtures/esquemas-cfdi.xsd`, a wrapper that imports every CFDI schema (CFDI 4.0, Pagos 2.0, Carta Porte 3.1, implocal, leyendasFisc and Timbre Fiscal Digital 1.1), so a template may carry supported complements. The CFDI 4.0 original string (`cadenaoriginal_4_0.xslt`) does not include the stamp, so a template may carry a `tfd:TimbreFiscalDigital` whose `SelloCFD` is `{{SELLO}}`.
- Fixture `Fixtures/Cfdi40/ingreso-timbrado.xml` (template `tools/fixtures/plantillas/Cfdi40/ingreso-timbrado.xml`): `ingreso-flete` with folio 1025 and a synthetic stamp (UUID `E3B5F1A2-7C4D-4E8F-9A1B-2C3D4E5F6A7B`, `RfcProvCertif` `SPR190613I52`, `NoCertificadoSAT` `30001000000500003456`, fake `SelloSAT`). It is not a real PAC stamp.

## 23. Pagos 2.0 (verified)

- `src/DanmaobCfdi.Core/Reading/Pagos20/`, namespace `DanmaobCfdi.Core.Reading.Pagos20`, every property `{ get; internal init; }` with the exact SAT names:
    - `TotalesData`: the ten optional totals (`decimal?`, `TotalRetencionesIVA` to `TotalTrasladosBaseIVAExento`) and `MontoTotalPagos` (`decimal`).
    - `DoctoRelacionadoData`: `IdDocumento`, `Serie?`, `Folio?`, `MonedaDR`, `EquivalenciaDR` (`decimal?`, up to 10 decimals without loss), `NumParcialidad` (`int`), `ImpSaldoAnt`, `ImpPagado`, `ImpSaldoInsoluto` (`decimal`) and `ObjetoImpDR`.
    - `PagoData`: `FechaPago` (`DateTime` without time zone), `FormaDePagoP`, `MonedaP`, `TipoCambioP` (`decimal?`), `Monto`, `NumOperacion?` and `DoctosRelacionados` (`IReadOnlyList<DoctoRelacionadoData>`).
    - `PagosData`: `Version`, `Totales` (`TotalesData`, a new empty instance by default) and `Pagos` (`IReadOnlyList<PagoData>`).
    - `Pagos20Extractor` (`internal static`): `internal static PagosData? Extract(XElement comprobante)` reads the `pago20:Pagos` child of `cfdi:Complemento` and returns `null` when there is none. `NumParcialidad` is parsed with `int.Parse` and `CultureInfo.InvariantCulture`.
- Outside the model on purpose: the ordering and beneficiary accounts and banks (`RfcEmisorCtaOrd`, `NomBancoOrdExt`, `CtaOrdenante`, `RfcEmisorCtaBen`, `CtaBeneficiario`), the SPEI chain (`TipoCadPago`, `CertPago`, `CadPago`, `SelloPago`), and the taxes per document (`ImpuestosDR`) and per payment (`ImpuestosP`). Adding them later is a compatible change.
- `ComprobanteData.Pagos` (`PagosData?`) is the last property of `ComprobanteData`, after `TimbreFiscalDigital`, and `Cfdi40Extractor` fills it with `Pagos20Extractor.Extract(root)`.
- Fixture `Fixtures/Cfdi40/pago-parcialidad.xml` (template `tools/fixtures/plantillas/Cfdi40/pago-parcialidad.xml`): a CFDI of type `P` with one payment of 2264.00 MXN for two documents (the first installment of `ingreso-timbrado`, UUID `E3B5F1A2-7C4D-4E8F-9A1B-2C3D4E5F6A7B`, with its taxes; and a USD document with `EquivalenciaDR` `0.0543478261`), and a synthetic stamp.
- The comparison of the user's verification script accepts a trailing comma after the last assignment of an object initializer.

## 24. SAT catalogs: common base and simple CFDI 4.0 catalogs (verified)

- `src/DanmaobCfdi.Catalogs/Persistence/SatCatalogEntity.cs` (`internal abstract`): the four common columns of ADR-06, `Clave` (`string`), `Descripcion` (`string?`), `FechaInicioVigencia` and `FechaFinVigencia` (`DateOnly?`), with `{ get; set; }`. It is never an entity of the model by itself: each catalog is its own table.
- `src/DanmaobCfdi.Catalogs/Persistence/SatCatalogConfiguration.cs` (`internal static`): `internal const string DescriptionCollation = "Latin1_General_100_CI_AI"` and `ConfigureCatalog<TEntity>(EntityTypeBuilder<TEntity> builder, string tableName, int keyLength, int descriptionLength, bool descriptionRequired) where TEntity : SatCatalogEntity`, which maps the table, the key `Clave` with its length, `Descripcion` with its length, nullability and collation, and the two validity dates as `date`. Every catalog configuration calls it; never configure those four columns another way.
- Catalog entities and configurations live in `src/DanmaobCfdi.Catalogs/Cfdi40/`, namespace `DanmaobCfdi.Catalogs.Cfdi40`, both `internal sealed`: the entity is the SAT name without `c_` and inherits `SatCatalogEntity`; its configuration, `<Entity>Configuration`, implements `IEntityTypeConfiguration<Entity>`.
- Simple catalogs (only the four common columns), migration `Cfdi40SimpleCatalogs`; key length and description length: `c_Aduana` 2/100, `c_Exportacion` 2/100, `c_Meses` 2/50, `c_MetodoPago` 3/50, `c_ObjetoImp` 2/100, `c_Periodicidad` 2/50, `c_TipoRelacion` 2/100, `c_MotivoCancelacion` 2/100, `c_TipoCadenaPago` 2/50, and without description (`Descripcion` nullable) `c_TipoFactor` 6/50 and `c_PatenteAduanal` 4/50.
- Key lengths come from the enumerations of the SAT schemas; the user's database step compares them before applying a migration. Description lengths are the real maximum of the SAT Excel rounded up to 50, 100, 250, 500 or 1000.
- `tests/DanmaobCfdi.Tests/Catalogs/Cfdi40/SimpleCatalogModelTests.cs` checks every simple catalog with `GetRelationalModel()`, and that `HasPendingModelChanges()` is false: a configuration changed without its migration fails the tests.

## 25. SAT catalogs: CFDI 4.0 catalogs with their own structure (verified)

- Migration `Cfdi40OwnStructureCatalogs`. Entities and configurations in `src/DanmaobCfdi.Catalogs/Cfdi40/`, `internal sealed`, with `{ get; set; }` properties; key length / description length:
    - `c_RegimenFiscal` 3/100 and `c_UsoCFDI` 4/100: `AplicaPersonaFisica` and `AplicaPersonaMoral` (`bit`).
    - `c_UsoCFDI_RegimenFiscalReceptor` (class `UsoCFDIRegimenFiscalReceptor`, does not inherit `SatCatalogEntity`): `UsoCFDI` (4) and `RegimenFiscal` (3), composite key by name, and the only foreign key of the schema, to `c_UsoCFDI`, without navigation properties.
    - `c_FormaPago` 2/50: ten text columns (`Bancarizado`, `NumeroOperacion`, `RfcEmisorCuentaOrdenante`, `CuentaOrdenante`, `RfcEmisorCuentaBeneficiario`, `CuentaBeneficiario` and `TipoCadenaPago` of 50; `PatronCuentaOrdenante`, `PatronCuentaBeneficiaria` and `NombreBancoEmisorCuentaOrdenante` of 100), because they mix `Sí`, `No` and `Opcional`.
    - `c_Moneda` 3/100: `Decimales` (`int`) and `PorcentajeVariacion` (`decimal(5,2)`, nullable).
    - `c_Impuesto` 3/50: `Retencion` and `Traslado` (`bit`), `LocalOFederal` (50).
    - `c_TipoDeComprobante` 1/50: `ValorMaximo` (`decimal(24,6)`); Nómina (`N`) keeps the single value shared by `NS` and `NdS`.
    - `c_TasaOCuota` (does not inherit `SatCatalogEntity`): `Id` (`int` identity key), `Tipo` (10), `ValorMinimo` (`decimal(18,6)`, nullable), `ValorMaximo` (`decimal(18,6)`), `Impuesto` (50, the tax name as the Excel writes it), `Factor` (10), `Traslado` and `Retencion` (`bit`), and the validity dates as `date`; a unique index on `Tipo`, `ValorMinimo`, `ValorMaximo`, `Impuesto` and `Factor` declared with `HasFilter(null)`.
- Every `decimal` column declares its precision with `HasPrecision`; an entity that does not inherit `SatCatalogEntity` maps its own table, key and validity dates.
- Model tests read the design-time model (`context.GetService<IDesignTimeModel>().Model`): in `context.Model`, which is read-optimized, collations, precisions and filters are not available. `tests/DanmaobCfdi.Tests/Catalogs/Cfdi40/OwnStructureCatalogModelTests.cs` checks the eight tables.

## 26. SAT catalogs: geographic and large CFDI 4.0 catalogs (verified)

- `SatCatalogConfiguration` has two methods with the same five parameters: `ConfigureCatalog` (common columns AND the key `Clave`) and `ConfigureColumns` (the same common columns WITHOUT any key). A catalog with a composite key calls `ConfigureColumns` and then `HasKey` with its two column names; never call `ConfigureCatalog` and then `HasKey` again, because the first key could stay as an alternate key.
- Migration `Cfdi40GeographicCatalogs`. Entities and configurations in `src/DanmaobCfdi.Catalogs/Cfdi40/`; key length / description length:
    - `c_Pais` 3/100: `FormatoCodigoPostal` (50), `FormatoRegistroIdentidadTributaria` (100), `ValidacionRegistroIdentidadTributaria` (50), `Agrupaciones` (50), all nullable.
    - Composite keys: `c_Estado` (`Clave` 3, `Pais` 3) /50; `c_Municipio` (`Clave` 3, `Estado` 3) /100; `c_Localidad` (`Clave` 2, `Estado` 3) /50; `c_Colonia` (`Clave` 4, `CodigoPostal` 5) /100.
    - `c_CodigoPostal` 5/50 without description: `Estado` (3), `Municipio` (3, nullable), `Localidad` (2, nullable), `EstimuloFranjaFronteriza` (`tinyint`), `HusoHorario` (50), and for summer and winter `MesInicioHorario…` (20), `DiaInicioHorario…` (20), `HoraInicioHorario…` (5), nullable, and `DiferenciaHoraria…` (`int`).
    - `c_ClaveProdServ` 8/250: `IncluirIvaTrasladado` and `IncluirIepsTrasladado` (50), `ComplementoQueDebeIncluir` (100, nullable), `EstimuloFranjaFronteriza` (`tinyint`), `PalabrasSimilares` (1000, nullable, collation `Latin1_General_100_CI_AI`).
    - `c_ClaveUnidad` 3/250 (`Descripcion` holds the unit name): `DescripcionUnidad` (1000), `Nota` (250), `Simbolo` (50), all nullable.
    - `c_NumPedimentoAduana` (does not inherit `SatCatalogEntity`): `Id` (`int` identity key), `Aduana` (2), `Patente` (4), `Ejercicio` and `Cantidad` (`int`), validity dates as `date`, unique index on `Aduana`, `Patente` and `Ejercicio`.
- Relations between geographic catalogs (`c_CodigoPostal` to estado, municipio and localidad; `c_Colonia` to código postal) are plain columns without foreign keys; they are validated in code.
- `tests/DanmaobCfdi.Tests/Catalogs/Cfdi40/GeographicCatalogModelTests.cs` checks the nine tables, including that each composite-key table has no unique key other than its primary key.

## 27. SAT catalogs: simple Carta Porte 3.1 catalogs (verified)

- Carta Porte 3.1 entities and configurations live in `src/DanmaobCfdi.Catalogs/CartaPorte31/`, namespace `DanmaobCfdi.Catalogs.CartaPorte31`; tests in `tests/DanmaobCfdi.Tests/Catalogs/CartaPorte31/`, namespace `DanmaobCfdi.Tests.Catalogs.CartaPorte31`. Same shapes as the CFDI 4.0 catalogs.
- Simple catalogs (only the four common columns), migration `CartaPorte31SimpleCatalogs`; key length / description length: `c_CveTransporte` 2/50, `c_TipoEmbalaje` 4/250, `c_SectorCOFEPRIS` 2/50, `c_FormaFarmaceutica` 2/50, `c_CondicionesEspeciales` 2/50, `c_TipoMateria` 2/50, `c_DocumentoAduanero` 2/250, `c_ParteTransporte` 4/50, `c_FiguraTransporte` 2/50, `c_RegistroISTMO` 2/50, `c_ConfigMaritima` 3/50, `c_ClaveTipoCarga` 3/50, `c_ContenedorMaritimo` 5/50, `c_TipoDeTrafico` 4/50, `c_SubTipoRem` 6/50, and without description (`Descripcion` nullable) `c_NumAutorizacionNaviero` 15/50.
- `tests/DanmaobCfdi.Tests/Catalogs/CartaPorte31/SimpleCartaPorteCatalogModelTests.cs` checks the sixteen tables with one theory, against the design-time model.

## 28. SAT catalogs: Carta Porte 3.1 catalogs with their own structure and the tariff catalog (verified)

- Migration `CartaPorte31OwnStructureCatalogs`. Entities and configurations in `src/DanmaobCfdi.Catalogs/CartaPorte31/`, except `c_FraccionArancelaria`, the only catalog of `src/DanmaobCfdi.Catalogs/ComercioExterior/` (namespace `DanmaobCfdi.Catalogs.ComercioExterior`). Key length / description length:
    - `c_ClaveProdServCP` 8/250: `PalabrasSimilares` (1000, nullable, collation `Latin1_General_100_CI_AI`) and `MaterialPeligroso` (50: `0`, `1` or `0,1`).
    - `c_MaterialPeligroso` 5/500 (the XSD has a key of 5 characters, although the Excel only reaches 4): its SAT key repeats, so it calls `ConfigureColumns`, has the key `Id` (`int` identity) and a NON-unique index on `Clave`; `ClaseODivision` (50), `PeligroSecundario` (50) and `NombreTecnico` (250), all nullable.
    - `c_ClaveUnidadPeso` 3/250 (`Descripcion` holds the unit name): `DescripcionUnidad` (1000), `Nota` (250), `Simbolo` (50), nullable, and `Bandera` (50).
    - `c_Estaciones` 12/100: `ClaveTransporte` (2), `Nacionalidad` (50), `DesignadorIATA` and `LineaFerrea` (50, nullable).
    - `c_TipoEstacion` 2/50 and `c_TipoPermiso` 6/250: `ClaveTransporte` (50, a list of keys as text); `c_RegimenAduanero` 3/250: `ImpoExpo` (50).
    - `c_ConfigAutotransporte` 8/250: `NumeroEjes` (`int`, nullable), `NumeroLlantas` (50, nullable, text such as `8 o 10`) and `Remolque` (50).
    - `c_CodigoTransporteAereo` 5/100 (`Descripcion` holds the airline name): `Nacionalidad` and `DesignadorOACI` (50).
    - `c_TipoDeServicio` 4/50 and `c_TipoCarro` 4/50: `Contenedor` (`bit`); `c_Contenedor` 4/50: `TipoContenedor` (50).
    - `c_DerechosDePaso` 6/50: `Entre`, `Hasta` and `Concesionario` (250), `OtorgaRecibe` (50).
    - `c_FraccionArancelaria` 10/2000 (the only description of 2000): `UMT` (2).
- With this migration all 57 CFDI 4.0, Carta Porte 3.1 and tariff catalogs have their table; the 4 Retenciones 2.0 catalogs remain.
- `tests/DanmaobCfdi.Tests/Catalogs/CartaPorte31/OwnStructureCartaPorteCatalogModelTests.cs` checks the fourteen tables, including `c_FraccionArancelaria`.
