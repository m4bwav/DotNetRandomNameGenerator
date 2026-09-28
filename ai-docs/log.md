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
- plans/2026-09-28-retrofit-and-2.3.0-release.md (D1-D16, one question); decisions/2026-09-28-retrofit-without-code-changes.md (proposed).
## [2026-09-27] index | rebuilt (6 entries)
## [2026-09-27] index | rebuilt (6 entries)

## [2026-09-28] update | Phase 1 ruled: recommendations stand, place list fixed in place as 2.3.0
- Maintainer: "follow all recommendations but fix the place-list bug now"; asked the shape (in place 2.3.0, 3.0.0, or a new name), chose in place as 2.3.0; then "I consider it a bug, the old behavior wasn't worth preserving".
- `dotnet run --project tools/CensusTools -c Release -- place places2k.txt out`: 16,969 names, no duplicates, no classification words left, no U+FFFD; 150 truncations removed, 246 names restored (Georgetown, Felicity, Middletown); one-letter "Y" is a real Alaska CDP.
- Provenance of the person lists: the 1990 files from www2.census.gov/topics/genealogy/1990surnames rebuilt with the old logic (first column, Title case, first occurrence) equal the shipped lists exactly (88,799, 1,219, 4,275).
- Source SHA-256: places2k.zip 520a8374...bbc9, places2k.txt f3a49faf...e92e, dist.all.last b0e2b374...f4e1, dist.male.first 0a5078ef...470b, dist.female.first bd2f310f...b358.

## [2026-09-28] add | Retrofit Phase 2: golden replay, canary, place list, workflows, docs
- tests/RandomNameGeneratorLibrary.GoldenTests (xunit.v3, the capture's Cases.cs, Fixtures.cs and Json.cs by link): against the unchanged library, net10.0 green on the first build; net48 had 16 differences, all the int.MaxValue OutOfMemoryException message ("Exception of type ..." in a 32-bit process versus "Array dimensions exceeded supported range." in the 64-bit capture); the replay's net48 RuntimeIdentifier is win-x64 since, then green. Commit 64ebdf3.
- Canary after that commit: `return names[RandGen.Next(0, names.Length - 1)]` in PersonNameGenerator.Pick: 156 cases red on each runtime; `git checkout -- RandomNameGeneratorLibrary/PersonNameGenerator.cs`: green. `git diff --exit-code 84dbc6b -- tests/Golden` empty.
- Place list regenerated with tools/CensusTools (commit 8bebf10). GOLDEN_WRITE_DIFFERENCES wrote Exceptions/2.3.0.places.net10.0.json and .net48.json: 35 cases each, all place-derived (Resources, Lists, seeded, Multiple, extensions, shared Random, Random calls with Next(0,16969), scripted bounds, subclass read); the replay checks every exception key names places and really differs from 2.2.0, and an oracle test recomputes the nine seeded cases as PlaceNames[new Random(seed).Next(0, 16969)].
- Templates adapted (ci.yml, release.yml, verify-published.yml, tests/consumers, Dependabot, .gitattributes eol=lf, the SDK .gitignore, audit switches), version 2.3.0-beta.1, baseline 2.2.0 (commit 3398d5c). Traps: the template's content check lists the nuspec before README.md but a C-locale sort puts README.md first for this id; zizmor 1.30.1 wants a Dependabot cooldown of 7 days (the template has 3); `{{` also matches GitHub's own expressions, so the placeholder check is `{{[A-Z_]+}}`; adopting eol=lf left CRLF working files from an autocrlf checkout (dotnet format ENDOFLINE) until `git rm -r --cached . && git reset --hard` after a commit.
- Local: restore locked, format, build 0 warnings, audit restore, 202 tests on net10.0 and net48, pack with validation against 2.2.0 (content list as ci.yml expects), consumers net10.0 and net48 green. Fresh clone of the pushed branch (4457d01): the same, green. actionlint 1.7.12 (release zip, checksum OK) clean; zizmor 1.30.1 --offline no findings (4 suppressed); check-workflow-shell.py exit 0; check-readme-images.mjs 3 images ok.
- README rows: the place row regenerated with `dotnet run sample.cs` (#:project, new Random(20260924)); the person row reproduced unchanged; ReadmeExamplesBehavior pins all eight rows.

## [2026-09-28] update | GitHub settings applied before the pull request (L-077)
- Rulesets 24095614 master (deletion, non-fast-forward, required check ci, admin bypass; copied from CachingServiceWithAOPSupport 24089709) and 24095615 Tags only by admins. Secret scanning and push protection on, private vulnerability reporting on, Dependabot security updates on, default workflow permissions read without PR approvals, delete branch on merge, wiki and projects off, homepage https://www.nuget.org/packages/RandomNameGeneratorLibrary; read back.
- GitHub Releases v2.0.0 and v2.0.1 created with --latest=false (changelog section, the nupkg from nuget.org); v2.2.0 stays Latest. Issue #7 comment 5862607663 (cause and the 2.1.0 fix).
## [2026-09-27] index | rebuilt (6 entries)
