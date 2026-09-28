# Handoff

## Current state
Retrofit of 2.2.0 to the package-modernize standard, branch `v2-retrofit` (2026-09-28). Phase 0 golden capture 84dbc6b (tests/Golden, never edit). Ruled: every recommendation, plus the place list fixed in place as 2.3.0. Phase 2 done and verified locally and from a fresh clone (202 tests, pack, consumers, actionlint, zizmor). GitHub settings applied (rulesets 24095614 master and 24095615 tags). Plan: plans/2026-09-28-retrofit-and-2.3.0-release.md.

## In progress
2.3.0 is released and verified (release run 36419884168, verify-published 36421937100 on three OSes, attestation, README images); PR #15 (baseline 2.3.0) merged as 65a3af8. Records done: package-modernization #10 merged (inventory row 4 done), package-modernize #10 merged (L-105, L-106, C-20260928-4).

GitHub wiki (2026-09-28): eleven pages published (wiki commit 0bf2dd6) from the sibling working copy `D:\m4bwa\Claude\Projects\Ai\labs\DotNetRandomNameGenerator.wiki`, remote `DotNetRandomNameGenerator.wiki.git`. How to update it: notes/2026-09-28-github-wiki.md.

## Decisions made this session
decisions/2026-09-28-retrofit-without-code-changes.md (accepted; the place-list part overruled: fixed now as 2.3.0).

## Dead ends hit
The net48 golden replay as win-x86 differs from the 64-bit capture in one OutOfMemoryException message; it runs as win-x64. Adopting eol=lf in an autocrlf clone leaves CRLF working files until `git rm -r --cached . && git reset --hard` after a commit.

## Standing work
Dependabot pull requests (seven-day cooldown); Mark's nuget.org deprecations of 1.1.0 (Critical bugs) and 1.0.5.1, 1.1.1, 1.2.0, 1.2.1 (Legacy, alternate 2.x); the next major (3.0) removes CensusListStripper and FileCompressor. README: the thread-safety sentence says a default generator is safe to share on .NET Framework, but there it holds a plain per-instance `Random`; reword at the next README change (the wiki already states one generator per thread).

## Evergreen upkeep (from the SessionStart hooks)
evergreen: no claims due; dandy: one undated claim, re-check only before relying on it; acestep-music refresh half done (research saved, edits and PR pending, see that repo's HANDOFF on branch refresh-2026-09-27).

## Next single action
Mark merges PR #16 (records only). Nothing else is owed by the agent for this package; the README thread-safety sentence waits for the next README change.
