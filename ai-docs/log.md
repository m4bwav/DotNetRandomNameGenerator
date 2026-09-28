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

## [2026-09-28] add | Phase 3: pull request #13, CI fixes, independent review and its fixes
- PR https://github.com/m4bwav/DotNetRandomNameGenerator/pull/13. CI run 36372933350 failed on both OSes: exit code 5, "Zero tests ran" (the golden project lacked Microsoft.Testing.Extensions.CodeCoverage for ci.yml's --coverage); fixed 35b7655. Run 36373102418: Windows green, Ubuntu exit 1 without a message in the package content check (grep on self-closing empty dependency groups under -e and pipefail); fixed 1e4e416 (checked locally with `bash -e -o pipefail` on the nupkg, and with a wrong expectation, which fails with its ::error line). Run 36373304184: green (Ubuntu, Windows, ci).
- Independent review (read-only subagent, prompts/review-subagent.md): differential of the 2.3.0-beta.1 nupkg against the published 2.2.0 over 5,000 seeds and all 60 call kinds, on net10.0 and net48: 144,891 comparisons per runtime, 0 differences outside the place list; every place answer equals PlaceNames[(int)(sample*count)] of its build; the data rebuilds exactly from the Census file (also by an independent suffix-only parser). 10 findings, all fixed: (1) the CI dependency check (already 1e4e416); (2) consumers could restore a nuget.org copy of the same version: run.sh now writes a nuget.config with packageSourceMapping (the package from the local folder only); (3) "every seeded place name differs" was false (2 percent of seeded draws land on the same name): CHANGELOG, release notes, plan and notes say "almost every"; (4) the exception regex also matched 26 recorded cases that must not change: the replay now pins the exact 35 keys; (5) the oracle covered 9 of 35 exceptions: it now covers all 35 (resource hash, list, seeded, large, multiple, extensions, Random calls, scripted bounds; the mixed seed-99 and seed-314 cases also compare every non-place answer with 2.2.0); (6) to (8) AGENTS.md: the consumers command, the win-x64 golden pin, the seven-day cooldown; (9) the beta's release notes now carry the substance and a link; (10) run.sh compares whole lines (grep -qxF).
- run.sh against nuget.org 2.2.0 fails as it should (the consumer sees the old place list).
- Machine fact: Windows Application Control ("An Application Control policy has blocked this file") refused to start the freshly rebuilt net48 golden test exe once; the net48 run is proven by CI's Windows job.

## [2026-09-28] add | Phase 4 and 5: merged, beta tagged, waiting at the approval gate
- Mark merged PR #13 as merge commit e3d610a (2026-09-28T03:52:36Z) and said he had edited the nuget.org Trusted Publishing policy (workflow file release.yml, environment nuget, package glob RandomNameGeneratorLibrary, scope: new versions only).
- ci on master run 36375417348: green (Ubuntu, Windows, ci).
- Tag v2.3.0-beta.1 on e3d610a pushed (the "Cannot create ref due to creations being restricted" line is the admin bypass message, L-087; ls-remote shows the tag on e3d610a).
- release.yml run 36375599177: build and test, Windows net48 and net10.0, attest all green; "push to nuget.org (after approval)" waiting at the nuget environment.
- PR #14 (branch release-2.3.0): version 2.3.0, changelog section dated 2026-09-28 and moved above the beta; checked locally (build, 103 tests on net10.0, pack, consumers of 2.3.0, notes extraction, dated-heading check). To be merged only after the beta is verified.

## [2026-09-28] update | Correction to the entry above
- Mark said "I merged everything"; he did not say whether the nuget.org policy edit (workflow ci.yml to release.yml) is done. The beta's push job will show it: a NuGet/login failure there means the policy still names ci.yml.
