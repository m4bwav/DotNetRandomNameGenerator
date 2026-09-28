# Changelog

All notable changes to RandomNameGeneratorLibrary. The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project uses [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [2.3.0] - 2026-09-28

2.3.0 answers every call as 2.2.0 did, on .NET Framework and on .NET, except for place names: `tests/Golden` holds 426 calls recorded from the published 2.2.0 on each runtime, and the golden tests replay them against every build. The 35 calls per runtime that draw from the place list are the one exception, pinned separately. No public type, member or parameter name changed.

### Fixed

- The place list is rebuilt from the Census 2000 `places2k.txt` with `tools/CensusTools`. Since 2014 it had cut a name at the first "town", "city", "CDP", "village", "municipality", "borough" or "(balance)" anywhere in it: Georgetown was listed as `George`, Hagerstown as `Hagers`, Felicity as `Feli`, and one-letter entries `G` and `H` existed. 150 entries that were not place names are gone and 246 names are back (Allentown, Georgetown, Middletown, Prophetstown, ...): 16,969 names, up from 16,873.

### Changed

- Seeded place names differ from 2.2.0 (`new PlaceNameGenerator(42)`, `new Random(42).GenerateRandomPlaceName()`) because the list changed: about 98 percent of them, and none can be relied on to stay the same. Seeded person and star names are unchanged. For the record: seeded place names also changed in 2.1.0, when duplicates were removed, which that release did not say; seeded person names are the same as in 1.2.2.

### Added

- Golden tests: every answer of the published 2.2.0 recorded per runtime (`tests/Golden`, captured from nuget.org) and replayed against each build; the public API with parameter and protected member names (`tests/Golden/PublicApi-2.2.0.txt`); a test that the README's example rows reproduce.
- `release.yml`: releases from a `v*` tag only when the tag equals the version and sits on `master`, tested on Linux and Windows, attested, pushed through Trusted Publishing after the maintainer's approval. `verify-published.yml` checks a release from nuget.org on Linux, Windows and macOS. CI packs with package validation against 2.2.0, checks the package's contents and runs fresh consumers of the packed package.
- `SECURITY.md`, `AGENTS.md`, a downloads badge, and `tools/CensusTools/SOURCES.md` with the Census files' addresses and hashes.

### Changed (build)

- Actions pinned to commit SHAs; publishing moved from `ci.yml` to `release.yml`; Dependabot also updates the SDK in `global.json`, groups test packages and waits seven days; line endings LF on every OS.

## [2.3.0-beta.1] - 2026-09-28

The release rehearsal of 2.3.0, published as a prerelease to prove the new release path (release.yml, the approval gate, verification from nuget.org). It has the same code as 2.3.0: the place list is rebuilt from the Census 2000 file, so names such as Georgetown are no longer cut to "George", and almost every seeded place name differs from 2.2.0. Full notes: the 2.3.0 section of https://github.com/m4bwav/DotNetRandomNameGenerator/blob/master/CHANGELOG.md

## [2.2.0] - 2026-09-27

### Added

- `StarNameGenerator` and `IStarNameGenerator` generate the names of real stars: `GenerateRandomStarName()` (an IAU proper name or a Bayer or Flamsteed designation, for example `Tau Ceti`), `GenerateRandomProperStarName()` (IAU names only), `GenerateRandomCatalogStarName()` (`HD` or `HIP` designations of 343,518 catalogued stars) and `GenerateMultipleStarNames(int)`, with the same default, `Random` and seed constructors as the other generators.
- `StarNameGenerator.ProperStarNames` (640), `DesignatedStarNames` (3,076), `StarNames` (both) and `CatalogStarNameCount`.
- `Random.GenerateRandomStarName()` and `Random.GenerateMultipleStarNames(int)` extensions.
- `tools/StarLists/build_star_lists.py` rebuilds the star resources from the IAU WGSN list, the Yale Bright Star Catalogue and the Hipparcos catalogue. The package grows by about 45 KB (5%).

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

[Unreleased]: https://github.com/m4bwav/DotNetRandomNameGenerator/compare/v2.3.0...HEAD
[2.3.0-beta.1]: https://github.com/m4bwav/DotNetRandomNameGenerator/compare/v2.2.0...v2.3.0-beta.1
[2.3.0]: https://github.com/m4bwav/DotNetRandomNameGenerator/compare/v2.2.0...v2.3.0
[2.2.0]: https://github.com/m4bwav/DotNetRandomNameGenerator/compare/v2.1.0...v2.2.0
[2.1.0]: https://github.com/m4bwav/DotNetRandomNameGenerator/compare/v2.0.1...v2.1.0
[2.0.1]: https://github.com/m4bwav/DotNetRandomNameGenerator/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/m4bwav/DotNetRandomNameGenerator/releases/tag/v2.0.0
[1.2.2]: https://www.nuget.org/packages/RandomNameGeneratorLibrary/1.2.2
