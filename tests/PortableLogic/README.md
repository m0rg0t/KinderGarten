# CSV import regressions

Run from the repository root with .NET SDK 10:

```sh
dotnet run --project tests/PortableLogic/PortableLogic.csproj --configuration Release
```

Links the actual BetterParserEngine, CsvParser and IParserEngine source. Covers empty fields, final quoted fields, escaped quotes, custom separators, CRLF endings, malformed quotes, dictionary mapping and concurrent calls on one engine. Includes the semicolon-separated import format used by MainPage. WinRT async operations are minimal Task adapters; cancellation, COM and the Windows UI are not emulated. No Azure writes or HTTP requests are made.

The harness has no NuGet dependencies, clears package sources, compiles with C# 5 syntax, and does not publish anything. CI runs the same command with read-only repository permissions and commit-pinned actions. A passing host test is **not a build or runtime validation of the historical application**. Both Windows Store 8.1 application projects remain unchanged.

## Dependency and application validation still needed

Application package manifests, checked-in binaries, project references and Windows Store 8.1 targets remain unchanged. The Windows 8.1 SDK, Bing Maps XAML SDK and VCLibs are unavailable on this host; whole application validation and per-target dependency modernization need that historical toolchain. Existing Dependabot PR #1 proposes a manifest-only Newtonsoft.Json upgrade and does not establish compatibility of the referenced DLLs. No migration to a different runtime is included.

## CSV limits

This fixes single-record field parsing; record splitting remains newline-based. Embedded newlines in quoted fields, quoted header names and headerless dictionaries are not implemented by this change. Invalid unterminated quotes or text after a closing quote now fail explicitly with FormatException instead of truncating silently. Blank records remain ignored. The live import service and its current data have not been contacted.
