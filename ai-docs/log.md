# Log

Append-only. One line per operation: `## [YYYY-MM-DD] op | title` where op is one of add, update, supersede, verify, verify-failed, prune, handoff, index. Newest at the bottom. Never edited, only appended; this is the history the entries themselves do not carry.

## [2026-09-27] init | scaffolded
## [2026-09-27] index | rebuilt (3 entries)

## [2026-09-28] add | Retrofit Phase 0: survey, baseline, golden capture, gap audit (Windows 11, SDK 9.0.317 and 10.0.401, gh)
- package-modernize at 3988316 (LEARNINGS consolidated, PR #8). Branch `v2-retrofit` from master ff0aa53.
- `survey-nuget.sh RandomNameGeneratorLibrary m4bwav/DotNetRandomNameGenerator` > notes/2026-09-28-survey.txt (runs survey-github.sh too). Findings in notes/2026-09-28-phase-0-gap-audit.md.
- Baseline: `dotnet restore --locked-mode`, `dotnet build --no-restore -c Release` 0 warnings, `dotnet test --no-build -c Release` 146 of 146 (net48 x86, net10.0 x64), `dotnet format --no-restore --verify-no-changes` exit 0, `dotnet package list --vulnerable --include-transitive` none. nuget-latest.py: xunit.v3 4.0.1, Microsoft.Testing.Extensions.CodeCoverage 18.11.2, Microsoft.NETFramework.ReferenceAssemblies 1.0.3 (all current, none in cooldown).
- check-readme-images.mjs --registry nuget on README.md and on the README inside the 2.2.0 nupkg (identical files): 2 images ok, exit 0.
- Golden capture tests/Golden/Capture (C#, split like CachingServiceWithAOPSupport's; RandomNameGeneratorLibrary [2.2.0] from nuget.org): `dotnet build -c Release`, then bin/Release/net48/Capture.exe and `dotnet bin/Release/net10.0/Capture.dll` twice each: 426 cases, byte-identical per runtime. assemblySha256 2398427f... (net48, lib/netstandard2.0) and 5709f134... (net10.0), equal to `sha256sum` of the nupkg's lib files. 356 of 426 answers equal across runtimes; the 70 others are exception wording, the default Random, FileCompressor bytes.
- tests/Golden/ApiList (reflection, net48 and net10.0 identical): PublicApi-2.2.0.txt, 101 lines, protected members and abstract or sealed kinds included (the worked example's lister skipped protected constructors and fields).
- Commit 84dbc6b: the recordings, the capture, the API list and `-text` for them in .gitattributes. From here on `git diff --exit-code 84dbc6b -- tests/Golden` must stay empty.
- Place-list check: Census 2000 places2k.txt from www2.census.gov/geo/docs/maps-data/data/gazetteer/places2k.zip; the 2014 stripper's logic reproduces the shipped list exactly (16,873 names, same order); a correct strip gives 16,969; 150 shipped entries are not names, 246 names missing.
- 1.2.2 on net48 (scratch project): seeded person names equal 2.2.0's for seeds 0, 1, 42, 12345; seeded place names all differ (2.1.0's dedup).
- everlast registered (mode repo, sync push); old ai-docs/log/*.md moved to notes/ and plans/modernization-plan.md to plans/2026-09-25-modernization-2.1.0.md with git mv; lint clean after rewording two false positives (a C# lazy type read as an opinion, the id-token permission read as a credential).
- AGENTS.md (template adapted, everlast block), CLAUDE.md (import line), .github/copilot-instructions.md.

## [2026-09-28] add | Retrofit Phase 1: plan and decision record, stop for rulings
- plans/2026-09-28-retrofit-and-2.2.1-release.md (D1-D16, one question); decisions/2026-09-28-retrofit-without-code-changes.md (proposed).
## [2026-09-27] index | rebuilt (6 entries)
## [2026-09-27] index | rebuilt (6 entries)
