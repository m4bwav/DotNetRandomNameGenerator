---
title: Phase 0 gap audit of RandomNameGeneratorLibrary 2.2.0 (package-modernize retrofit)
kind: note
date: 2026-09-28
verified: 2026-09-28
stale_after: never
tags: [survey, phase-0, retrofit, nuget, golden, gap-audit, place-data]
summary: "what 2.2.0 lacks against the package-modernize standard, what the golden capture recorded, the place-list truncation bug, and how seeded output moved from 1.2.2 to 2.2.0; read before planning a change or a release"
---

# Phase 0 gap audit: RandomNameGeneratorLibrary 2.2.0

## Summary

2.2.0 (2026-09-27) was modernized on 2026-09-24 and 25, before the package-modernize skill existed. Its code, targets and metadata are current; what it lacks is the release and verification machinery and a contract for its behaviour. The golden capture of the published 2.2.0 (426 cases per runtime, commit 84dbc6b) is that contract now. The audit found one real defect, in the data rather than the code: the place list still carries the 2014 stripper's truncations (150 entries are not place names, 246 real names are missing). The survey output is [2026-09-28-survey.txt](2026-09-28-survey.txt).

## Registry and repository facts (2026-09-28)

- Versions: 1.0.0 to 1.0.5 unlisted; 1.0.5.1 (2014, lib/net45), 1.1.0, 1.1.1, 1.2.0, 1.2.1 (2016), 1.2.2 (2016-10-17, lib/net40 and lib/netstandard1.6), 2.0.0, 2.0.1, 2.1.0 (2026-09-25), 2.2.0 (2026-09-27) listed. 3,887,857 downloads: 1.2.2 has 3,786,606, 1.0.5.1 67,776, every 2.x together about 280. Owner rogersm0, no deprecations, no vulnerabilities.
- Odd old packages (the survey now lists every version's files): 1.1.0 ships the source files, `obj/Debug` and `obj/Release` caches and no DLL, so it installs nothing; 1.1.1, 1.2.0 and 1.2.1 put the DLL in `lib/` with no target-framework folder. 2.0.0 to 2.2.0 are clean (lib per target, README, XML docs from 2.1.0, icon from 2.1.0).
- The 2.2.0 DLLs that answered the capture have SHA-256 2398427f... (lib/netstandard2.0, loaded on net48) and 5709f134... (lib/net10.0); both equal the files in the nuget.org nupkg.
- GitHub: 95 stars, 24 forks (none active since 2024-11), issues #5 and #7 closed, no open pull requests, only `master`, 0 alerts, 0 webhooks, no repository secrets (the `nuget` environment holds `NUGET_USER`, reviewer m4bwav, deployment rule tag `v*`), every security feature off, private vulnerability reporting off, default workflow permissions write, no ruleset or branch protection, `delete_branch_on_merge` false, homepage http://www.markdavidrogers.com/random-name-generator-net-library/. GitHub Releases exist for v2.1.0 and v2.2.0 only.
- Baseline as it is (Windows, SDK 10.0.401): `dotnet restore --locked-mode`, build 0 warnings, `dotnet test` 146 of 146 on net10.0 and net48, `dotnet format --verify-no-changes` clean, no vulnerable packages, direct test packages current (xunit.v3 4.0.1, Microsoft.Testing.Extensions.CodeCoverage 18.11.2, Microsoft.NETFramework.ReferenceAssemblies 1.0.3, checked with nuget-latest.py).
- README (repository and the one inside the 2.2.0 nupkg are identical): two images, NuGet version and CI badges, both live (`check-readme-images.mjs --registry nuget` exit 0). The standard row has three; the downloads badge is missing.

## Gaps against the package-modernize standard

| Item | 2.2.0 state | Standard |
|---|---|---|
| Golden capture | none before 84dbc6b; 2.1.0 changed every seeded place name unguarded | recording per runtime from the published version, replayed in CI |
| API guard | package validation against 2.0.1 | baseline 2.2.0, plus a PublicApi file with parameter and protected names |
| Workflows | one ci.yml that also publishes; actions on major tags; no final `ci` job | ci.yml, release.yml (tag equals version and sits on master, net48 tests, attestation, gated push without checkout), verify-published.yml; SHA pins; actionlint and zizmor clean |
| Consumers | none | tests/consumers/run.sh on the packed package in CI and on nuget.org after release |
| Audit | NuGetAudit defaults only | audit-pipeline restore step that fails on findings |
| Dependabot | nuget and github-actions, ungrouped, no cooldown | plus dotnet-sdk, grouped, cooldown |
| Rulesets and settings | none; everything off; workflow permissions write | master ruleset requiring `ci`, admins-only tag ruleset, scanning, push protection, private reporting, read permissions |
| SECURITY.md | missing | template |
| AGENTS.md, CLAUDE.md import, Copilot pointer | missing | added in Phase 0 |
| ai-docs | old layout (`log/` hidden by the VS .gitignore) | everlast, mode repo (done in Phase 0) |
| Icon | present since 2.1.0 (generated, the maintainer's blue style) | nothing to do |
| README badges | 2 of 3 | NuGet version, CI, downloads |

## What the golden capture recorded

Groups: embedded resources (names, byte counts, SHA-256, line endings), every static list (type, count, hash, first and last entries, read-only flag, U+FFFD count), seeded output of every method for nine seeds including 0, -1, int.MinValue and int.MaxValue, constructor equivalence, every `GenerateMultiple*` for counts 0, 1, 3, 25, 10000, -1, int.MinValue and int.MaxValue, every `Random` extension including null receivers, a `Random` shared between generators and the caller, the exact `Random` calls each method makes (a recording `Random` subclass), boundary picks through a scripted `Random` (first, last, out of range), default generators (membership, 8 threads), a caller's `BaseNameGenerator` subclass, four cultures, and the obsolete `CensusListStripper` and `FileCompressor`.

- Seeded names are identical on .NET Framework 4.8 and .NET 10 (`new Random(seed)` keeps its legacy algorithm on .NET). The 70 cases that differ between the two recordings are exception wording (.NET adds ` (Parameter 'x')`), the default `Random` (.NET 6+ shares `Random.Shared`; netstandard2.0 on .NET Framework makes one per generator), and `FileCompressor`'s compressed bytes (each runtime's zlib). Hence one recording per runtime; the replay needs no exceptions.
- Every method draws with `Random.Next(0, count)` only: one call per name, two for a gendered pick (`Next(0, 2) == 0` means male). Any other answer than 0 picks female, so a caller's `Random` subclass that returns 2 or -1 gets a female name; an out-of-range answer throws IndexOutOfRangeException, except in `GenerateRandomCatalogStarName`, where -1 gives "HD 0", a star that does not exist. Both only happen with a broken `Random`.
- `GenerateMultiple*` builds a `List<string>` eagerly with the requested capacity; int.MaxValue throws OutOfMemoryException ("Array dimensions exceeded supported range") at once.
- The static list properties return the internal `string[]` (typed `IReadOnlyList<string>`, `IsReadOnly` true through `ICollection<string>`), so a caller who casts to `string[]` can change every generator's list in the process.
- The obsolete `CensusListStripper` reads files as UTF-8 (a Latin-1 input gives U+FFFD: `Mu, U+FFFD, oz` in the recording), throws on a blank row, and cuts a place name at the first occurrence of a classification word anywhere in it ("Newtown CDP" becomes "New").

## The place-list defect (found by the audit, not by the capture)

The embedded `Resources.places2k.txt.stripped` is exactly what the 2014 library stripper produces from the Census 2000 `places2k.txt` (downloaded from www2.census.gov on 2026-09-28): the same 16,873 names in the same order, 2.1.0's 49 hand-fixed accents aside. That stripper looks for "town", "city", "CDP", "village", "municipality", "borough", "(balance)" as substrings anywhere in the name, so every name containing one of them is cut there: Georgetown becomes George, Hagerstown Hagers, Felicity Feli, Allentown Allen. 150 of the 16,873 entries are not place names (among them "G" and "H"), and 246 real names are missing (no place on the list ends in "town"). `tools/CensusTools` (2.1.0) matches the word with a leading space and would produce 16,969 correct names, but 2.1.0 hand-fixed the old list instead of regenerating it, so the tool and the data disagree. Changing the list changes every seeded place name.

## Seeded output from 1.2.2 to 2.2.0

A scratch program on the published 1.2.2 (net48, seeds 0, 1, 42, 12345, ten draws per method) against the 2.2.0 recording: every seeded person name is identical (first and last, male, female, last); every seeded place name differs, because 2.1.0 deduplicated the list (25,375 rows to 16,873). The 2.1.0 changelog says the list changed but not that seeded place names did; 1.2.2's 3.79 million downloads make that the upgrade note that matters.

Related: builds on [2026-09-25-stages-1-to-6.md](2026-09-25-stages-1-to-6.md); see also [2026-09-27-star-names-2.2.0.md](2026-09-27-star-names-2.2.0.md).
