---
title: GitHub wiki written and published for 2.3.0
kind: note
date: 2026-09-28
verified: 2026-09-28
stale_after: 2027-03-28
tags: [wiki, docs, 2.3.0, github]
summary: "the eleven wiki pages, where their git working copy is, the verification program and its saved output, how the wiki was published (the first page must be saved in the web UI before the wiki repository exists) and how to update it; read before touching the wiki or the README's thread-safety sentence"
---

# GitHub wiki for 2.3.0

## Summary

Mark asked for the repository wiki (https://github.com/m4bwav/DotNetRandomNameGenerator/wiki) to be filled with helpful material. Eleven pages were written from the 2.3.0 source, README, CHANGELOG, AGENTS.md, the ai-docs notes and the two closed issues, with every example output verified against the published 2.3.0 package (a .NET 10 file-based app with `#:package RandomNameGeneratorLibrary@2.3.0`, `dotnet fsi`, and PowerShell 7 `Add-Type`). Published on 2026-09-28 as wiki commit 0bf2dd6 after Mark saved the first page; every page answers 200 and the sidebar and footer render.

## Updating the wiki later

1. In the working copy (see below), `git pull --ff-only`, then edit the markdown. Page names are the file names with hyphens (`Getting-Started.md` is the page "Getting Started"); links between pages are plain markdown (`[Recipes](Recipes)`).
2. Re-verify: copy `2026-09-28-wiki-verify.cs` (next to this note) to a folder outside the repository. Change the version in its `#:package` line and in the F# and PowerShell snippets inside it (the `#r` line and the NuGet cache path). Run `dotnet run wiki-verify.cs > out.txt`; the F# and PowerShell cases need `dotnet fsi` and `pwsh` on the PATH. Normalise the output to LF and diff it with `2026-09-28-wiki-verify.out.txt`. The seeded values and list counts are the same on every run, and the unseeded examples print only after a check that they are on their list, so every difference is a behaviour change and a page to fix. Save the new output over the old.
3. `python <wikiwright>/scripts/wikiwright.py outputs <wiki dir> out.txt` must report 0 missing. That needs wikiwright 0.3.0 or later: 0.2.0 could not read values shown in comments and checked none of this wiki's outputs. Then `wikiwright.py check <wiki dir> --version <new>` and the everwrite checker.
4. Commit, `git push`, then `wikiwright.py live m4bwav/DotNetRandomNameGenerator <wiki dir>`.

When a release changes counts, seeded examples or the version number, the pages to touch are Home, Getting started, API reference, Recipes, Reproducible names, Versions and upgrading, and the footer's version line.

## Where the pages are

`D:\m4bwa\Claude\Projects\Ai\labs\DotNetRandomNameGenerator.wiki` (a sibling of this clone, outside this repository), branch `master`, remote `origin` = `https://github.com/m4bwav/DotNetRandomNameGenerator.wiki.git`, commit 0bf2dd6. Files: `Home.md`, `Getting-Started.md`, `API-Reference.md`, `Recipes.md`, `Reproducible-Names.md`, `Name-Lists-and-Data-Sources.md`, `Versions-and-Upgrading.md`, `FAQ.md`, `Development.md`, `_Sidebar.md`, `_Footer.md`. Plain markdown links (`[Recipes](Recipes)`), no wikilinks. Everwrite checker: 0 strong, 4 weak (all judged fine).

## How it was published

The first `git push` answered `Repository not found`: the repository had `has_wiki: true` but no page had ever been created, and GitHub creates the `.wiki.git` repository only when the first page is saved in the web UI. There is no REST or GraphQL API for wiki pages, so an agent cannot do that step. Mark saved a first page; `git push --force -u origin master` then replaced GitHub's placeholder commit (8543ca6) with the eleven pages. For any other repository's wiki: run `git ls-remote https://github.com/<owner>/<repo>.wiki.git` first, and ask for the click early when it fails.

## Facts verified while writing (not in the README)

- `new PersonNameGenerator(42)`: `Alisa Streets`; male `Morton`, female `Marguerita`, last `Vis`. `new PlaceNameGenerator(42)`: `Boardman`. `new StarNameGenerator(42)`: star `Delta² Gruis`, proper `Parumleo`, catalogue `HIP 4210`. `new Random(42)` chained: female `Marguerita`, place `Sedco Hills`, star `Ran`.
- Two place names contain a comma (`Islamorada, Village of Islands`, `Lynchburg, Moore County`); 33 place names and 1 proper star name contain an apostrophe; no name contains a double quote. Person names are letters only.
- 331 first names are on both the male and the female list; 6,394 place names contain a space; 52 place names carry non-ASCII characters; 1,394 last names start with `Mc`.
- The lists are in Census frequency order (Smith, Johnson, Williams; James, John, Robert; Mary, Patricia, Linda).
- Package 2.3.0: nupkg 942,766 bytes; netstandard2.0 DLL 995,840 bytes; the seven resources total 977,150 bytes (last names 695,422).
- The unit tests have no test that shares one generator between threads; the golden capture's "8 threads" case is the only multi-thread evidence.

## Brought under the saved-output rule (2026-09-28)

The wiki was written by hand before wikiwright 0.2.0 and kept no verification program. wikiwright's 0.3.0 session wrote `2026-09-28-wiki-verify.cs` from the skill's NuGet template. It prints every output the pages show: exactly for seeded calls, and for the unseeded examples on Home and Getting started each value only after checking it is on the list its method draws from. It runs the F# and PowerShell snippets as written, and prints the counts and names the pages state in prose. Its output, identical over two runs, is `2026-09-28-wiki-verify.out.txt`.

- wikiwright 0.2.0's `outputs` saw none of this wiki's 28 outputs. They are shown as comments (`// "Marguerita"`, `// always "Alisa Streets"`, F# `// Boardman`, PowerShell `# 88799`) and as comment lines that close a code block, and 0.2.0 read only `//=>`. 0.3.0 reads them.
- Every seeded value, list count and name on the pages matched the run, with one exception. Recipes said "The longest last name is `Hollingsworth`". In fact 250 last names have 13 letters, the most any has, and some of them look cut off at that length (`Christopherso`); two place names share the longest length, 35 characters. The sentence now says so.
- Page changes (wiki commit f0bb65b): that sentence; the three `// Marguerita` comments on Reproducible names are quoted so the checker reads them; the `dotnet run names.cs` fence is tagged `sh`. Result: `outputs` 28 checked, 0 missing; `check` clean; everwrite 0 strong on the changed page.
- Not run: .NET Framework (the cross-runtime claims rest on the repository's golden tests, which replay on net48 in CI), Unity, and the ASP.NET Core and CSV recipes, which show no output.

## Open item found: README thread-safety sentence

The README says a default-constructed generator "is safe to share between threads (it uses `Random.Shared` on .NET 6+ and a well-seeded `Random` per generator on .NET Framework)". On the netstandard2.0 build `BaseNameGenerator` creates a plain `new Random(SeedSource.Next())` per generator, and `System.Random` is not thread-safe, so the .NET Framework half of that sentence overstates it. The wiki states the safe pattern (one generator per thread on .NET Framework). Fix the README sentence in the next README change; the README ships in the package, so it reaches nuget.org with the next release.

## Gotchas

- GitHub wiki repositories use branch `master`; `git init -b master` matched it.
- `curl` of the nuget.org `registration5-gz-semver2` index needs `--compressed`; without it the body is gzip bytes.

Related: builds on [2026-09-28-phase-0-gap-audit.md](2026-09-28-phase-0-gap-audit.md); see also [../HANDOFF.md](../HANDOFF.md), [../../README.md](../../README.md).
