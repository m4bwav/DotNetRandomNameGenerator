# Handoff

## Current state
Retrofit of 2.2.0 to the package-modernize standard, branch `v2-retrofit` (2026-09-28). Phase 0 golden capture 84dbc6b (tests/Golden, never edit). Ruled: every recommendation, plus the place list fixed in place as 2.3.0. Phase 2 done and verified locally and from a fresh clone (202 tests, pack, consumers, actionlint, zizmor). GitHub settings applied (rulesets 24095614 master and 24095615 tags). Plan: plans/2026-09-28-retrofit-and-2.3.0-release.md.

## In progress
2.3.0 is released and verified (release run 36419884168, verify-published 36421937100 on three OSes, attestation, README images). PR from branch baseline-2.3.0 sets PackageValidationBaselineVersion to 2.3.0; waits for Mark's merge.

## Decisions made this session
decisions/2026-09-28-retrofit-without-code-changes.md (accepted; the place-list part overruled: fixed now as 2.3.0).

## Dead ends hit
The net48 golden replay as win-x86 differs from the 64-bit capture in one OutOfMemoryException message; it runs as win-x64. Adopting eol=lf in an autocrlf clone leaves CRLF working files until `git rm -r --cached . && git reset --hard` after a commit.

## Lessons still to file in package-modernize (L-105 and up)
- The agent logged a maintainer statement he had not made (the policy edit); corrected in log.md. Rule: log only what the maintainer said, quoted.
- Any lesson from the beta and 2.3.0 release runs.

## Evergreen upkeep (from the SessionStart hooks)
evergreen: no claims due; dandy: one undated claim, re-check only before relying on it; acestep-music refresh half done (research saved, edits and PR pending, see that repo's HANDOFF on branch refresh-2026-09-27).

## Next single action
Mark merges the baseline-2.3.0 PR. Then Phase 7: inventory row 4 as done in package-modernization, its PR #10 out of draft and merged; lessons L-105 and up in package-modernize (log only what the maintainer said, quoted; check-readme-images defaults to npm, pass --registry nuget).
