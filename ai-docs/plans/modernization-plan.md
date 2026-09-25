# RandomNameGeneratorLibrary modernization plan

Written 2026-09-25 from a full read of the repo at 2.0.1 (commit `7d1162c`). Meant to be executed in fresh sessions, one stage per session or PR. Tick boxes as work lands. Ship stages 1 to 5 as **2.1.0** (additive only); anything marked BREAK waits for **3.0**.

Repo: https://github.com/m4bwav/DotNetRandomNameGenerator. Package: https://www.nuget.org/packages/RandomNameGeneratorLibrary. Release procedure: bump `<Version>` in `RandomNameGeneratorLibrary/RandomNameGeneratorLibrary.csproj`, push a `v<version>` tag, approve the `nuget` deployment in Actions (Trusted Publishing, see README "Building and releasing").

## Already good, do not redo

- csproj: `netstandard2.0;net10.0`, `LangVersion latest`, `Deterministic`, `ContinuousIntegrationBuild` on GitHub, `PublishRepositoryUrl`, `EmbedUntrackedSources`, `snupkg` symbols, `PackageLicenseExpression`, `PackageReadmeFile`, `PackageTags`, repository metadata. SourceLink works (2.0.0 nuspec carries the commit).
- Embedded resources have explicit `LogicalName`.
- CI: least-privilege permissions, `id-token: write` only on publish, `environment: nuget`, `NuGet/login@v1`, `--skip-duplicate`, `fetch-depth: 0`, current action majors.
- `.slnx` solution; xunit 2.9.3, Test.Sdk 18.0.1.
- Null and negative-count guards on the person generator and its extensions.
- Person lists are clean: LF, ASCII, Title-case, no blanks or duplicates (88,799 last / 4,275 female / 1,219 male).

## Stage 1: data fixes and integrity tests (S, no API break, output changes)

Facts verified 2026-09-25:

| File | Lines | Unique | Notes |
| --- | --- | --- | --- |
| `Resources.places2k.txt.stripped` | 25,375 | 16,873 | Franklin and Clinton appear 27 times each (one row per state in the Census file); 49 lines contain U+FFFD (`Bayam�n zona urbana`) from a Latin-1 decode error |

- [ ] Fix the 49 Puerto Rico names. Re-strip from the original Census 2000 places file with `Encoding.Latin1`, or hand-fix the lines (á, é, í, ó, ú, ñ, ü).
- [ ] Decide duplicates: either dedupe the place file (rare names get likelier, file ~130 KB smaller) or keep it and change the README sentence "every entry is equally likely" to say places are weighted by how many states have them. Recommendation: dedupe, and say so in the changelog.
- [ ] Add resource integrity tests: no blank lines, no U+FFFD, no duplicates, expected counts. These would have caught both problems.
- [ ] Add `.gitattributes` with `* text=auto` and `*.stripped text eol=lf` so the resources stay LF on Windows checkouts.

## Stage 2: runtime hygiene (S, no API break)

- [ ] `BaseNameGenerator.cs` lines 25 to 31: delete the `#if NET40` block; `GetTypeInfo()` is unnecessary on both targets.
- [ ] `BaseNameGenerator.ReadResourceByLine` lines 36 to 47: dispose the stream and reader with `using`; throw `InvalidOperationException` naming the resource when `GetManifestResourceStream` returns null.
- [ ] `PersonNameGenerator.InitNames` (144 to 156) and `PlaceNameGenerator.InitPlaceNames` (42 to 48): unsynchronised `if (x == null) x = ...`. Replace with `static readonly Lazy<string[]>` per list so each file is parsed once and only when first used.
- [ ] `BaseNameGenerator` line 15 `new Random()`: on .NET Framework it is time-seeded, so two generators made in the same tick repeat each other. Use `Random.Shared` under `#if NET6_0_OR_GREATER`, and on netstandard2.0 a process-wide seeded `Random` behind a lock to hand out seeds.
- [ ] Document that a generator holding a caller-supplied `Random` is not thread-safe (one generator per thread), in XML docs and README.
- [ ] `RandomPlaceNameExtensions.cs` lines 8 to 16: add the same null and negative guards the person extensions have, with the right parameter name.

## Stage 3: build settings, docs, package metadata (M, no API break)

- [ ] Write XML docs on every public member. The packed `RandomNameGeneratorLibrary.xml` is currently empty (151 bytes) because of `NoWarn CS1591`. Then remove that NoWarn.
- [ ] `Nullable` enable. The public API has no null returns, so this only adds annotations.
- [ ] `TreatWarningsAsErrors` true, `EnableNETAnalyzers` true, `AnalysisLevel latest`. Fix what surfaces.
- [ ] `EnablePackageValidation` true with `PackageValidationBaselineVersion` 2.0.1 to catch accidental API breaks on the netstandard2.0 surface.
- [ ] Add `IsTrimmable` and `IsAotCompatible` on the net10.0 target (the only reflection is `GetManifestResourceStream`, which is safe).
- [ ] Add `CHANGELOG.md` (Keep a Changelog format, entries for 1.2.2, 2.0.0, 2.0.1, 2.1.0) and feed `PackageReleaseNotes` from it; the csproj note still says only "2.0.0: rebuilt".
- [ ] Add `PackageIcon` (a small PNG in the package), `Copyright` with a year range, `NeutralLanguage en`. Update `LICENSE` year (2014 today). Remove the redundant `GenerateAssemblyInfo` line.
- [ ] Add `global.json` pinning SDK 10 with `rollForward: latestFeature`.
- [ ] README: fix "contains a stripped down lists" and "exercised has an extension"; add a Data section (1990 Census name files, Census 2000 places, counts per list, Title-case), a thread-safety note, a Unity note (netstandard2.0, no dependencies, no runtime Regex), and mention `IPersonNameGenerator` and `IPlaceNameGenerator` for DI and mocking. Note that `GenerateRandomFirstName` picks gender 50/50 first, so any one male name is about 3.5 times likelier than any one female name.

## Stage 4: CI (S each)

- [ ] `.github/dependabot.yml` for `github-actions` and `nuget`, weekly.
- [ ] `setup-dotnet` with `cache: true` plus `packages.lock.json` (`RestorePackagesWithLockFile`).
- [ ] `concurrency` group on pull requests to cancel superseded runs.
- [ ] `dotnet format --verify-no-changes` step; add an `.editorconfig` if formatting is noisy.
- [ ] Coverage: `--collect:"XPlat Code Coverage"` and upload the report as an artifact.
- [ ] Publish job: fail if the tag does not equal `<Version>`; after the push, create a GitHub Release with the nupkg and snupkg attached (`softprops/action-gh-release`, needs `contents: write` on that job only).
- [ ] Test the netstandard2.0 build for real: add `net48` to the test project's `TargetFrameworks` and a Windows runner to the matrix (M).

## Stage 5: additive API (S each, ship in 2.1.0)

- [ ] Expose the lists: `IReadOnlyList<string> MaleFirstNames`, `FemaleFirstNames`, `LastNames` on the person generator and `PlaceNames` on the place generator, so callers can count or pick their own.
- [ ] Seed convenience constructors: `new PersonNameGenerator(int seed)`, `new PlaceNameGenerator(int seed)`.
- [ ] Collapse the six copy-pasted loops in `PersonNameGenerator` lines 62 to 132 into one private `Generate(int count, Func<string> pick)`.
- [ ] Mark `BaseNameGenerator` `[EditorBrowsable(EditorBrowsableState.Never)]` and document that it is not for use; sealing it is a BREAK.
- [ ] Mark `CensusListStripper` and `FileCompressor` `[Obsolete]` with a message pointing at a new `tools/` console project that holds them and is not packed. Note `CensusListStripper` has a latent bug: `IndexOf(".")` can return -1 and `Remove(-1)` throws. Removing the classes is a BREAK for 3.0.

## Stage 6: tests (M)

- [ ] Fix `PersonNameGeneratorBehavior` line 22: `Assert.Equal(expected, actual)` order is reversed. Seed the "WithoutRepeats" test (line 18) so it is deterministic. Move `ShouldGenerateSameNameIfSameRandomGenerator` from `PlaceNameBehavior` to the person tests.
- [ ] Add cases: `GenerateMultiple*(0)` returns empty, negative count throws, male-only and female-only methods draw from the right list (check membership in the exposed lists from stage 5), `GenerateMultiplePlaceNames`, extension methods are deterministic with a seed, null `Random` throws `ArgumentNullException`.
- [ ] Optional: xunit v3 on Microsoft.Testing.Platform.

## Deferred and breaking (3.0 candidates)

- Weighted selection. The frequency columns were stripped from the person lists, so this means re-stripping the 1990 files keeping cumulative frequency (about twice the resource size) and adding a `GenerateWeighted*` family or a constructor option. L.
- Deflate the embedded resources (about 990 KB of text makes each DLL about 1 MB; the nupkg compresses it anyway). Only worth it if someone complains about assembly size.
- Remove `CensusListStripper`, `FileCompressor`; make `BaseNameGenerator` internal; seal the generators.

## Related

- Site consumer: `m4bwav/markdavidrogers-web` references this package (2.0.1 as of PR #10); its `random_name` MCP tool and `/api/random-name` use `PersonNameGenerator` and `PlaceNameGenerator`.
- Sibling library plan: `DotNetJsonPrettyPrinter/ai-docs/plans/modernization-plan.md`.
