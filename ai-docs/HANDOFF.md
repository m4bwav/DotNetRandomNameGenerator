# Handoff

## Current state
Retrofit of 2.2.0 to the package-modernize standard, branch `v2-retrofit` (2026-09-28). Phase 0 golden capture 84dbc6b (tests/Golden, never edit). Ruled: every recommendation, plus the place list fixed in place as 2.3.0. Phase 2 done and verified locally and from a fresh clone (202 tests, pack, consumers, actionlint, zizmor). GitHub settings applied (rulesets 24095614 master and 24095615 tags). Plan: plans/2026-09-28-retrofit-and-2.3.0-release.md.

## In progress
Phase 3: the independent review (with a differential against the published 2.2.0) and the pull request; then the stop for the maintainer's review.

## Decisions made this session
decisions/2026-09-28-retrofit-without-code-changes.md (accepted; the place-list part overruled: fixed now as 2.3.0).

## Dead ends hit
The net48 golden replay as win-x86 differs from the 64-bit capture in one OutOfMemoryException message; it runs as win-x64. Adopting eol=lf in an autocrlf clone leaves CRLF working files until `git rm -r --cached . && git reset --hard` after a commit.

## Next single action
After the maintainer merges: the maintainer edits the nuget.org Trusted Publishing policy (workflow file ci.yml to release.yml), then tag v2.3.0-beta.1 on green master and stop for the approval.
