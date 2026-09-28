# AGENTS.md

Rules for any AI agent (Claude Code, Copilot, Cursor, Codex) working in this repository. `CLAUDE.md` and `.github/copilot-instructions.md` only point here.

## What this is

The NuGet package `RandomNameGeneratorLibrary` (namespace `RandomNameGeneratorLibrary`): random person and place names from US Census lists and real star names, with no dependencies. About 3.9 million downloads, almost all of 1.2.2 (2016); 2.x since 2026-09-25. Library in `RandomNameGeneratorLibrary/`, unit tests in `RandomNameGeneratorUnitTests/`, the golden contract in `tests/Golden/` and its replay in `tests/RandomNameGeneratorLibrary.GoldenTests/`, build-time tools in `tools/`. Start with `ai-docs/HANDOFF.md`; the current plan is the newest file in `ai-docs/plans/`.

## Rules

- **The same names for the same seed.** A caller who seeds a generator (`new PersonNameGenerator(42)`, `new PlaceNameGenerator(new Random(42))`, `new Random(42).GenerateRandomPlaceName()`) gets exactly the names the published 2.2.0 gave, on .NET Framework and on .NET, and every other recorded answer stays the same. The proof is `tests/Golden/2.2.0.*.json`, recorded from the nuget.org package by `tests/Golden/Capture` and replayed by the golden tests; `tests/Golden/PublicApi-2.2.0.txt` guards names and parameter names, package validation guards the API shape. Never edit or regenerate a recording. A fix that would change a recorded answer (a name list, the order of `Random` calls, an exception) goes under a new name with a decision entry in `ai-docs/decisions/` and a changelog line; the old name stays exact until the next major. The embedded name lists are data: changing a line changes seeded output.
- **Targets.** The library multi-targets `netstandard2.0` and `net10.0`: no net5+ APIs (`ArgumentNullException.ThrowIfNull`, `HashCode`) without an `#if` or a polyfill. The net10.0 build is trim and AOT compatible.
- **Tests cover every artifact.** Golden, unit, package validation at pack time, the consumers in `tests/consumers/run.sh` (packed package in CI, nuget.org in `verify-published.yml`). A behaviour change lands with its test. Tests never touch the network.
- **Nothing reaches nuget.org without the maintainer.** No API key is stored anywhere; `release.yml` publishes through Trusted Publishing from a job that waits at the `nuget` environment for the maintainer's approval. Never push a package from a machine.
- **Releases follow one ritual.** Update `CHANGELOG.md` (a release heading carries its date), set `<Version>` in `RandomNameGeneratorLibrary/RandomNameGeneratorLibrary.csproj`, merge, wait for `ci` to be green on `master`, then tag `v<version>` and push the tag. `release.yml` builds, tests, packs, attests, waits for the approval, pushes and creates the GitHub Release. Then run `verify-published` with the version. Tag only after green: a tag on a failing commit burns the version number, because tags are not force-pushed here.
- **Dependencies.** The library has none; keep it that way. Lock files are committed (`RestorePackagesWithLockFile`); run plain `dotnet restore` after changing a PackageReference and commit the lock file; CI restores with `--locked-mode`. Dependabot opens weekly pull requests (nuget, dotnet-sdk, github-actions); merge when `ci` is green. Actions are pinned to commit SHAs; keep it that way. New package versions wait three days before use.
- **Research beats recall.** SDK, package and action versions change; re-verify any version older than three months.
- **Document for handoff.** Anything learned, decided or built goes into `ai-docs/` before you finish; rewrite `ai-docs/HANDOFF.md` when work is left unfinished.
- **No AI attribution anywhere.**
- **Line endings.** Files are LF (`.gitattributes` and `.editorconfig`), so `dotnet format` agrees on every OS; the `*.stripped` resources and the golden files must stay byte for byte as they are.

## Commands

```
dotnet restore --locked-mode
dotnet format --verify-no-changes
dotnet build -c Release
dotnet test -c Release                              # net10.0 and net48 (net48 executes only on Windows)
dotnet restore -p:AuditPipeline=true --force        # fails on any NuGetAudit finding, as CI does
dotnet pack RandomNameGeneratorLibrary -c Release -o artifacts   # package validation against PackageValidationBaselineVersion
bash tests/consumers/run.sh artifacts               # fresh consumers of the packed package
```

## Layout and traps

- `RandomNameGeneratorLibrary/Resources.*.stripped` are the embedded lists (LogicalName `RandomNameGeneratorLibrary.Resources.<file>`). `tools/CensusTools` and `tools/StarLists/build_star_lists.py` rebuild them; a rebuild changes seeded output and is a golden-contract decision, not a chore.
- `tests/Golden/2.2.0.net48-windows.json` and `2.2.0.net10.0-windows.json` were captured from the published 2.2.0 by the program in `tests/Golden/Capture` (whose empty `Directory.Build.*` files keep this repository's MSBuild settings out). Seeded names are the same on both runtimes; exception messages, the default `Random` and `FileCompressor`'s bytes differ, so each runtime has its own recording.
- A locked-mode lock file for a multi-OS matrix must not depend on anything the SDK infers per OS: `Microsoft.NETFramework.ReferenceAssemblies` is referenced explicitly with `PrivateAssets="all"`, and the net48 test exe pins `RuntimeIdentifier win-x86` with `SelfContained false`.
- The Visual Studio `.gitignore` ignores every `log/` folder; everlast's `ai-docs/log.md` is a file and is unaffected.
- The publish job has no checkout, so it pins `dotnet-version` instead of reading `global.json`.
- After a release, set `PackageValidationBaselineVersion` to the released version.

## everlast (session knowledge, load on demand)

- `ai-docs/INDEX.md` lists what past sessions learned here (solutions with verified commands, decisions with reasons, plans). At the start of a task, scan it and open only the entries whose title or tags match; no line matches: `everlast.py search "<key terms>"` before concluding nothing was recorded. Read `ai-docs/HANDOFF.md` when continuing unfinished work (everlast-resume skill).
- Before acting on an entry marked `(recheck due)`, run `everlast.py recheck <entry>`, re-run its Verified-by command only when that is read-only or safe (a build, a test, a version query), then record `everlast.py verify <entry>` or `verify <entry> --failed "what broke"`; a fix that changed is superseded, never reused blindly.
- Before finishing a task that hit a dead end, verified a non-obvious command, made a design choice, or taught you something about the user, record it (everlast-capture skill, or `everlast.py note` / `handoff`); rewrite `HANDOFF.md` when work is left unfinished. Say "nothing to record" when that is true.
- Anything naming a person, an internal host or name, a credential, or an opinion about people goes to the private sidecar (`--private`), never here. Lessons about the user or this machine go to the user tier (`--user`).
- Rules go in this file, system layout in CODEMAP.md; the doc set holds only what could not be re-derived from the code in a minute.
- Link documents together with relative markdown links: every markdown folder is reachable from an index whose lines say when to read each file (`ai-docs/INDEX.md` is generated from frontmatter; give entries a one-line `summary`), and an entry links the entries it relates to on a typed `Related:` line (`supersedes`, `contradicts`, `builds on`, `see also`). The set then reads as a graph for people in Obsidian and for agents alike. No wikilinks in the repo.
