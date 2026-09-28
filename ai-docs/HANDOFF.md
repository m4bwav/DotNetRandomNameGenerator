# Handoff

## Current state
Retrofit of 2.2.0 to the package-modernize standard, branch `v2-retrofit` (2026-09-28). Phase 0 golden capture 84dbc6b (tests/Golden, never edit). Ruled: every recommendation, plus the place list fixed in place as 2.3.0. Phase 2 done and verified locally and from a fresh clone (202 tests, pack, consumers, actionlint, zizmor). GitHub settings applied (rulesets 24095614 master and 24095615 tags). Plan: plans/2026-09-28-retrofit-and-2.3.0-release.md.

## In progress
PR #13 merged (e3d610a). Tag v2.3.0-beta.1 pushed; release run 36375599177 waits at the `nuget` environment for Mark's approval (Actions, the run, Review deployments). PR #14 (release-2.3.0) sets 2.3.0 with a dated changelog; merge only after the beta is verified.

## Decisions made this session
decisions/2026-09-28-retrofit-without-code-changes.md (accepted; the place-list part overruled: fixed now as 2.3.0).

## Dead ends hit
The net48 golden replay as win-x86 differs from the 64-bit capture in one OutOfMemoryException message; it runs as win-x64. Adopting eol=lf in an autocrlf clone leaves CRLF working files until `git rm -r --cached . && git reset --hard` after a commit.

## Next single action
After Mark approves the beta: wait for both nuget.org indexes, run `gh workflow run verify-published.yml -f version=2.3.0-beta.1` and check it on three OSes, `gh release view v2.3.0-beta.1`, `gh attestation verify` on the run's nupkg with `--format json` (L-088). Then Mark merges PR #14; after ci is green on master, tag v2.3.0, stop for the approval, verify-published 2.3.0, `check-readme-images.mjs` on the README inside the 2.3.0 nupkg, then a PR setting PackageValidationBaselineVersion to 2.3.0. Finally: package-modernization PR #10 (records: inventory row 4 done, kickoff status done) out of draft and merged, and the everlast wrap-up.
