---
title: package-modernize retrofit and the 2.3.0 release
kind: plan
status: active
date: 2026-09-28
verified: 2026-09-28
stale_after: never
tags: [v2, retrofit, plan, nuget, github-actions, golden, release]
summary: "the living plan that brings 2.2.0 (modernized before the package-modernize skill existed) up to the skill's standard: gap audit, golden contract, decisions D1-D16 as ruled, the place-list fix, workflows, settings, the 2.3.0 release through release.yml, phases 0-7 with checkboxes"
---

# Retrofit and 2.3.0 release plan: RandomNameGeneratorLibrary

RandomNameGeneratorLibrary was modernized on 2026-09-24 and 25 ([the 2.1.0 plan](2026-09-25-modernization-2.1.0.md)), before the package-modernize skill existed, and shipped 2.0.0 to 2.2.0. This plan is a retrofit, not a rewrite: it adds the contract, the release path and the settings the skill requires, and changes no library code unless the maintainer rules otherwise. It follows the package-modernize skill (SKILL.md, references/nuget.md) at 3988316. Evidence goes to [../log.md](../log.md); the audit is [../notes/2026-09-28-phase-0-gap-audit.md](../notes/2026-09-28-phase-0-gap-audit.md).

## Status

Active. Phase 0 done 2026-09-28 (golden commit 84dbc6b on branch `v2-retrofit`). Phase 1 ruled 2026-09-28: every recommendation stands except D2, D3 and D4, which the maintainer overruled: "fix the place-list bug now", in place, as 2.3.0 ("I consider it a bug, the old behavior wasn't worth preserving"). Phase 3 done: pull request #13 green (CI run 36373549611), the independent review's 10 findings fixed (355af8b). At the pull request stop.

## Goal

- Every answer the published 2.2.0 gives stays the same, proven on every build by a golden replay of 426 recorded cases per runtime, a public API list and package validation against 2.2.0.
- Releases go through a separate, gated `release.yml` (tag equals the csproj version and sits on green master, net48 tests, attestation, maintainer approval) and are checked from nuget.org by `verify-published.yml` on three OSes.
- The repository is protected (rulesets, scanning, private reporting, read-only workflow token) and documented for any agent (AGENTS.md, everlast ai-docs).
- The place list is regenerated from the Census 2000 source with tools/CensusTools (16,969 names; the 2014 truncations gone); almost every seeded place name changes (about 98 percent), as the one ruled exception to the golden contract.
- 2.3.0 proves the new path, after a 2.3.0-beta.1 rehearsal.

## Where it stands (survey 2026-09-28)

| Fact | Value | Evidence |
|---|---|---|
| Published version, downloads | 2.2.0 (2026-09-27); 3,887,857 total, 3,786,606 of them 1.2.2; all 2.x about 280 | survey note |
| Source, build, tests | netstandard2.0 and net10.0, SDK 10.0.401, nullable, analyzers, xunit.v3 on MTP; 146 of 146 pass on net48 and net10.0; format clean | log, Phase 0 |
| Entry points | `PersonNameGenerator`, `PlaceNameGenerator`, `StarNameGenerator` (default, `Random` and seed constructors), their interfaces, `Random` extensions; obsolete `CensusListStripper`, `FileCompressor`; public abstract `BaseNameGenerator` | tests/Golden/PublicApi-2.2.0.txt (101 lines) |
| Runtime dependencies | none | nuspec |
| Issues, pull requests, forks | #5 and #7 closed; none open; 24 forks, none ahead in use | survey |
| Alerts, webhooks, secrets, security features | 0, 0, environment secret NUGET_USER only; every feature off; workflow permissions write | survey |
| Dead services | none (Gitter badge went in 2.0.x) | survey |
| README images | NuGet and CI badges live; downloads badge missing | check-readme-images.mjs, exit 0 on both READMEs |
| Leaked credentials | none found | survey, history of the workflow files |
| Golden capture | 426 cases per runtime, net48 and net10.0, twice each, byte-identical; DLL hashes equal the nupkg's | 84dbc6b |

## What the audit found, confirmed, and what the retrofit does

1. **The place list truncates names at "town", "city" and the other classification words** (150 non-names such as "Feli", "Hagers", "G"; 246 real names missing, such as Allentown and Georgetown). Present since 2014; 2.1.0 hand-fixed accents and duplicates but did not regenerate the list. The fix changes almost every seeded place name (about 98 percent, the review measured). Retrofit: keep (D3), document in the README and CHANGELOG, fix at 3.0 or under a new name.
2. **2.1.0 changed every seeded place name** relative to 1.x and 2.0 (the list was deduplicated) with nothing guarding it, and the changelog does not say so; seeded person names are identical from 1.2.2 to 2.2.0. Retrofit: a changelog line under 2.2.1 that states it, and the golden replay guards it from now on.
3. **The list properties hand out the internal arrays** (`(string[])PersonNameGenerator.LastNames` can be changed by a caller for the whole process). Retrofit: keep (a wrapper is a behaviour change for anyone who casts), document; 3.0 candidate.
4. **`GenerateRandomCatalogStarName` trusts `Random`**: a subclass answering -1 gets "HD 0". Keep; a broken `Random` is the caller's.
5. **Old packages that install nothing**: 1.1.0 (sources and obj caches, no DLL), 1.1.1 to 1.2.1 (DLL outside a target-framework folder). Retrofit: deprecation on nuget.org by the maintainer (D15).

## Decisions (recommendation first; the maintainer rules in the plan review, silence means the recommendation stands)

| # | Question | Recommendation | Why | Alternative |
|---|---|---|---|---|
| D1 | The compatibility promise | Every recorded 2.2.0 answer stays the same on its runtime: tests/Golden/2.2.0.net48-windows.json on net48, 2.2.0.net10.0-windows.json on net10.0 (Windows, Linux, macOS); no named exceptions; PublicApi-2.2.0.txt lines all present; package validation baseline 2.2.0 | Seeded output is what callers rely on; 2.1.0 changed it unguarded once | Compare net10.0 with the net48 recording plus message exceptions (more exceptions for nothing) |
| D2 | Library code changes | **Ruled: no code change; the place-list data is regenerated (D3).** Recommended was: none. Only the csproj's version, release notes and baseline change | The audit found no code bug; the one defect is data (D3) | Small cleanups (the analyzer set of the template) that touch code: no, each would need the replay to prove it |
| D3 | The place-list truncation | **Ruled 2026-09-28: fix in place in 2.3.0** (Resources.places2k.txt.stripped regenerated with tools/CensusTools; PlaceNames, every place method and extension use it; the place-derived golden cases are the one named exception, their new answers pinned in tests/RandomNameGeneratorLibrary.GoldenTests/Exceptions/2.3.0.places.*.json). Recommended was: keep the list in 2.x; README "Known issues" and a CHANGELOG line; regenerate with tools/CensusTools at 3.0 (together with the other 3.0 breaks) | Any fix changes almost every seeded place name, the exact thing the promise protects; 3.0 already has a list of breaks | (b) 2.3.0 adds the corrected list under a new name (for example a `PlaceNameGenerator.CensusPlaceNames` list with a constructor option); (c) fix in place in 2.3.0 as 2.1.0 did: rejected, it breaks the promise |
| D4 | Release | **Ruled: 2.3.0** (the place-list fix, README with the third badge and a regenerated place row, CHANGELOG, baseline 2.2.0, release notes), after a 2.3.0-beta.1 rehearsal. Recommended was 2.2.1 without the fix | nuget.org shows the README from the package, so the doc fixes need a version; proves release.yml and the policy before a real change depends on them | Wait for the next real change (the new path stays unproven) |
| D5 | Package validation baseline | 2.2.0 | The last published stable; 2.1.0 and 2.2.0 were additive | Keep 2.0.1 (misses the 2.1 and 2.2 additions) |
| D6 | Workflows | ci.yml, release.yml, verify-published.yml from the templates; publish removed from ci.yml; actions pinned to SHAs; final `ci` job; audit-pipeline step; pack content check; consumers on the packed package; actionlint and zizmor clean | The skill's CI-proven set (CachingServiceWithAOPSupport, 2026-09-27) | Keep ci.yml and add SHA pins only |
| D7 | Trusted Publishing policy | The maintainer edits the existing policy on nuget.org: Workflow File `ci.yml` becomes `release.yml`; owner m4bwav, repository DotNetRandomNameGenerator, environment `nuget`, package RandomNameGeneratorLibrary unchanged. After the merge, before the beta tag | The policy names one workflow file; release.yml is the only one that may publish | Add a second policy and delete the old one after the rehearsal |
| D8 | Tests added | tests/RandomNameGeneratorLibrary.GoldenTests (xunit.v3, the capture's Cases.cs, Fixtures.cs and Json.cs by link, net10.0 and net48); a PublicApi test in the unit tests; tests/consumers from the template; a README-examples test that the eight example rows reproduce | Replay and API list are the contract; the README rows are seeded and would drift silently | NUnit as in the template: two test stacks in one repository for nothing |
| D9 | Layout | Keep the project folders at the root (RandomNameGeneratorLibrary/, RandomNameGeneratorUnitTests/); new test projects under tests/ | A retrofit; moving the projects breaks links and history for no caller benefit | src/ and tests/ like the template |
| D10 | Build settings | Directory.Build.props gains the template's audit-pipeline switches and Source Link and symbol settings it lacks; the library's analyzer level stays as it is | New analyzer rules could demand code edits (D2) | The full template props |
| D11 | Dependabot | The template: nuget, github-actions and dotnet-sdk weekly, grouped, 7-day cooldown | Current file has no SDK updates and no grouping | Keep |
| D12 | README | Add the downloads badge; "Known issues" (D3, the list arrays); a sentence that seeded place names changed in 2.1.0; keep the example rows (now tested) | Three live badges; honest docs | Leave the README as it is |
| D13 | SECURITY.md | From the template (private vulnerability reporting, supported: 2.x) | Missing | None |
| D14 | GitHub settings | In the one question below | L-077: applied before the pull-request stop | |
| D15 | Old versions on nuget.org | The maintainer deprecates 1.1.0 ("Critical bugs": installs nothing) and 1.0.5.1, 1.1.1, 1.2.0, 1.2.1 ("Legacy", alternate RandomNameGeneratorLibrary 2.x); 1.2.2 stays undeprecated (the last net40 build, 3.79 million downloads, callers on .NET Framework 4.0 to 4.6.1 have no 2.x) | Broken packages should say so; the working 1.2.2 has callers 2.x cannot serve | Deprecate every 1.x as Legacy; or nothing |
| D16 | Dependents | markdavidrogers-web uses PersonNameGenerator and PlaceNameGenerator (2.1.0 or later); nothing changes for it; its seeded place output is guarded from now on | | |

## The one question for the maintainer (GitHub writes and nuget.org actions)

1. Rulings on D1 to D16 (silence keeps the recommendations).
2. GitHub settings to apply through `gh` before the pull request goes up: master ruleset (deletion and non-fast-forward blocked, required check `ci`, admin bypass); tag ruleset (only admins create, update or delete tags); secret scanning, push protection and private vulnerability reporting on; Dependabot security updates on; default workflow permissions read and no pull-request approvals by Actions; delete branches on merge; wiki and projects off (both empty).
3. Repository homepage: change http://www.markdavidrogers.com/random-name-generator-net-library/ to https://www.nuget.org/packages/RandomNameGeneratorLibrary (the overlay's default), or keep the site page? Ruled: the nuget.org page.
4. Create the missing GitHub Releases for v2.0.0 and v2.0.1 from the changelog, with the nupkgs from nuget.org attached?
5. A short comment on closed issue #7 saying 2.1.0 fixed its cause (default generators made in the same tick)?
6. Deletions: none needed now; after the merge, delete branch `v2-retrofit` (automatic with delete-on-merge).
7. On nuget.org, by you: the policy edit of D7 (after the merge, before the beta tag), and the deprecations of D15 (any time).

## Build and package specifics

Unchanged: csproj targets, metadata, resources, icon, README packing. Changed: `<Version>` 2.2.1-beta.1 then 2.2.1, `PackageValidationBaselineVersion` 2.2.0, `PackageReleaseNotes`. Directory.Build.props per D10. New: `tests/RandomNameGeneratorLibrary.GoldenTests/`, `tests/consumers/`, .github/workflows/release.yml and verify-published.yml, .github/zizmor.yml if needed, SECURITY.md. The golden replay project and the capture are the only places the golden files are read.

## Phases

### Phase 0: survey, baseline, golden capture (2026-09-28, no library code changed)
- [x] Survey (`survey-nuget.sh`, which runs `survey-github.sh`) into ai-docs/notes/2026-09-28-survey.txt; every version's files read
- [x] Baseline: restore locked, build 0 warnings, 146 of 146 tests, format clean
- [x] Golden capture of the published 2.2.0 on net48 and net10.0, twice each, byte-identical; PublicApi-2.2.0.txt (commit 84dbc6b; from here on these files do not change)
- [x] everlast (mode repo, sync push), old ai-docs migrated; AGENTS.md, CLAUDE.md (the AGENTS.md import line), Copilot pointer
- [x] Gap audit, place-list check against the Census 2000 source, 1.2.2 comparison
### Phase 1: plan
- [x] This plan and the decision record. Ruled 2026-09-28 (see Rulings).
### Phase 2: retrofit on branch v2-retrofit
- [x] Golden replay project: net10.0 green on the first build; net48 differed only by the process bitness of the harness (fixed by win-x64); canary after committing (156 red, reverted, green); golden files unchanged since 84dbc6b (64ebdf3)
- [x] Place list regenerated (D3), 35 place exceptions per runtime pinned and checked by an oracle (8bebf10)
- [x] PublicApi and README-examples tests; consumers; workflows from the templates, SHA-pinned, actionlint and zizmor clean; Dependabot; Directory.Build.props; SECURITY.md; README; CHANGELOG; version 2.3.0-beta.1 (3398d5c, 4457d01)
- [x] Verified locally and from a fresh clone of 4457d01 (202 tests, pack, consumers)
- [x] Pushed; pull request #13 with a "For review" list
### Phase 3: review
- [x] Independent read-only review including a differential against the published 2.2.0 (144,891 comparisons per runtime, 0 differences outside the place list); 10 findings fixed (1e4e416, 355af8b); summary on the pull request
- [x] Rulesets and security settings applied (L-077): rulesets 24095614 and 24095615, scanning, push protection, private reporting, read-only workflow token, homepage; Releases v2.0.0 and v2.0.1; issue #7 comment.
- [x] **Stop** for the pull request review.
### Phase 4: CI, merge, cleanup
- [x] CI green; the maintainer merges; merge SHA and method read back
### Phase 5: rehearsal
- [x] The maintainer edits the Trusted Publishing policy (D7). **Stop.**
- [x] Tag v2.3.0-beta.1 on green master; **stop** for the approval; verify-published on three OSes
### Phase 6: release
- [x] CHANGELOG dated, version 2.3.0, merged, green, tagged; **stop** for the approval; verify-published; GitHub Release; baseline stays 2.2.0 until 2.3.0 is published, then 2.3.0
### Phase 7: wrap-up
- [ ] HANDOFF around standing work; inventory row 4; the skill's retrofit path and lessons (L-090 and up) by pull request; the kickoff's "what the run found wrong"

## Test strategy

| Layer | What it proves | How | Runs where |
|---|---|---|---|
| Golden replay | Every 2.2.0 answer, per runtime | Capture's Cases.cs by link, compared as parsed JSON | net10.0 on Ubuntu and Windows, net48 on Windows |
| Public API | Names, parameter names, protected members | PublicApi-2.2.0.txt lines all present, none added without a decision | unit tests |
| Package validation | API shape against 2.2.0 | `dotnet pack` | CI pack step |
| Unit | Existing 146 tests | xunit.v3 | both runtimes |
| README examples | The published example rows reproduce | parse README table, regenerate with `new Random(20260924)` | unit tests |
| Consumers | The packed and the published package install and answer | tests/consumers/run.sh | CI (packed), verify-published (nuget.org, three OSes) |
| Differential | New build equals 2.2.0 on random seeds | review subagent, scratch project | local, Phase 3 |

## Security

No leaked credentials; no webhooks; no repository secrets. Publishing: Trusted Publishing bound to release.yml and environment `nuget` with the maintainer as required reviewer; the push job has no checkout and only first-party actions; tags limited to admins. Workflows: contents read by default, id-token set to write only in the push job, `persist-credentials: false`, SHA pins, zizmor. The library reads only its embedded resources, except the obsolete `CensusListStripper` and `FileCompressor`, which read and write caller-given paths (build tools, to be removed at 3.0).

## Badges and images: disposition

| Image or badge | What it shows now | Decision | New URL or reason |
|---|---|---|---|
| NuGet version (shields.io) | live | keep | |
| CI (GitHub Actions badge, ci.yml) | live | keep | |
| Downloads | missing | add | https://img.shields.io/nuget/dt/RandomNameGeneratorLibrary.svg |

## Verification checklist

| Claim | Command or place | Expected |
|---|---|---|
| Behaviour kept | golden replay in `dotnet test` | 426 cases equal on each runtime |
| Golden files untouched | `git diff --exit-code 84dbc6b -- tests/Golden` | empty |
| Canary | planted line in PersonNameGenerator.cs | replay red, then green after revert |
| API | PublicApi test, `dotnet pack` validation against 2.2.0 | green, no CP errors |
| Workflows | actionlint, zizmor, check-workflow-shell.py | clean |
| Release path | v2.2.1-beta.1 through release.yml | waits at `nuget`, pushes after approval, GitHub Release, attestation |
| Registry | verify-published on Ubuntu, Windows, macOS | both indexes, signature, consumers |
| README | check-readme-images.mjs on the repository README and the one in 2.2.1's nupkg | exit 0 |

## Risks and open points

- Editing the policy's workflow file may restart nuget.org's seven-day temporary state; the beta rehearsal shows it either way.
- FileCompressor's compressed bytes on Linux come from .NET's bundled zlib-ng, as on Windows; if the Ubuntu replay disagrees, the difference becomes a named exception decided from the CI run.

## Rulings (2026-09-28)

"Follow all recommendations but fix the place-list bug now"; asked how, the maintainer chose in place as 2.3.0 over 3.0.0 or a new name, and added "I consider it a bug, the old behavior wasn't worth preserving". So: the settings of question 2 are applied, the homepage moves to nuget.org, Releases for v2.0.0 and v2.0.1 are created, issue #7 gets its comment, and the agent merges its own skill and records pull requests (question 8, recommendation: yes).

## Next single action

Phase 2: the golden replay project, green against the unchanged 2.2.0 code, then the canary, then the regenerated place list with its named exception.
