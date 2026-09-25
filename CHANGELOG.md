# Changelog

All notable changes to RandomNameGeneratorLibrary. The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project uses [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [2.1.0] - 2026-09-25

### Fixed

- 49 Puerto Rico place names (for example `Bayamón zona urbana`, `Mayagüez zona urbana`) carried the U+FFFD replacement character from a Latin-1 file decoded as UTF-8. They now carry their accents.
- The place list had one row per state, so `Franklin` and `Clinton` were each 27 times likelier than a name found in one state. The list is now one entry per distinct name (16,873, down from 25,375 rows) and every place is equally likely, as the README always said.
- On .NET Framework, `new PersonNameGenerator()` and `new PlaceNameGenerator()` created in the same clock tick produced identical names because `new Random()` is time-seeded there ([#7](https://github.com/m4bwav/DotNetRandomNameGenerator/issues/7)). Default generators now use `Random.Shared` on .NET 6+ and a process-wide seeded source on netstandard2.0.
- The embedded lists were loaded with an unsynchronised null check; they are now `Lazy<string[]>`, parsed once per process on first use, thread-safe.
- Resource streams were never disposed; a missing resource now throws `InvalidOperationException` naming it instead of `ArgumentNullException` from `StreamReader`.
- `Random.GenerateRandomPlaceName()` and `Random.GenerateMultiplePlaceNames()` now throw `ArgumentNullException` / `ArgumentOutOfRangeException` like the person extensions.

### Added

- `PersonNameGenerator(int seed)` and `PlaceNameGenerator(int seed)` for reproducible names without constructing a `Random`.
- `PersonNameGenerator.MaleFirstNames`, `FemaleFirstNames`, `LastNames` and `PlaceNameGenerator.PlaceNames` as read-only lists, so callers can count or pick their own.
- XML documentation on every public member (the packed `.xml` was empty before), nullable annotations, .NET analyzers, trimming and AOT compatibility on net10.0, package validation against 2.0.1, package icon.
- `CHANGELOG.md`, `global.json`, `.gitattributes` (resources stay LF), `.editorconfig`, Dependabot, formatting and coverage in CI, net48 tests on Windows, a GitHub Release per tag.
- Tests moved to xunit.v3 4.0.1 on Microsoft.Testing.Platform (the library itself has no package dependencies).
- Resource integrity tests: counts, no blanks, no U+FFFD, no duplicates, Title case.
- `tools/CensusTools`, a console project holding the maintained stripping tool (reads Census files as Latin-1, dedupes).

### Deprecated

- `CensusListStripper` and `FileCompressor` are marked `[Obsolete]`; they were build-time tools and will be removed in 3.0. `BaseNameGenerator` is hidden from IntelliSense (`EditorBrowsable(Never)`) and documented as not for use.

## [2.0.1] - 2026-09-24

### Changed

- README shows example generated names and the sample code compiles ([#5](https://github.com/m4bwav/DotNetRandomNameGenerator/issues/5)).
- Published through nuget.org Trusted Publishing instead of a stored API key.

## [2.0.0] - 2026-09-24

### Changed

- Rebuilt for `netstandard2.0` and `net10.0` with the current SDK; `net40` and `netstandard1.6` targets dropped. SourceLink, deterministic build, README in the package. No API changes.

## [1.2.2] - 2018

- Last release of the 1.x line, targeting `net40` and `netstandard1.6`.

[Unreleased]: https://github.com/m4bwav/DotNetRandomNameGenerator/compare/v2.1.0...HEAD
[2.1.0]: https://github.com/m4bwav/DotNetRandomNameGenerator/compare/v2.0.1...v2.1.0
[2.0.1]: https://github.com/m4bwav/DotNetRandomNameGenerator/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/m4bwav/DotNetRandomNameGenerator/releases/tag/v2.0.0
[1.2.2]: https://www.nuget.org/packages/RandomNameGeneratorLibrary/1.2.2
