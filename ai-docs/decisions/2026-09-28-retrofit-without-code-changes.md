---
title: Retrofit 2.2.0 to the package-modernize standard without changing library code
kind: decision
status: proposed
date: 2026-09-28
verified: 2026-09-28
stale_after: never
tags: [retrofit, golden, place-data, release, 2.2.1]
summary: "why the retrofit adds a golden contract, workflows and settings but no library code, why the place-list truncation waits for 3.0, and why 2.2.1 ships anyway; read before changing a name list or releasing"
---

# Retrofit without code changes

## Context

2.2.0 was modernized before the package-modernize skill existed. The gap audit (see the Phase 0 note) found the code sound and the machinery missing, plus one data defect: the place list truncates names that contain a classification word (Georgetown is "George").

## Decision (proposed; the maintainer rules in the plan review)

- The published 2.2.0's recorded answers are the contract (one recording per runtime); no library code changes in the retrofit.
- The place list stays as it is in 2.x; the defect is documented and fixed at 3.0 (regenerated with tools/CensusTools), because any fix changes every seeded place name.
- 2.2.1 ships with docs, metadata and build changes only, after 2.2.1-beta.1, to prove release.yml and the policy change.

## Reasons

- The golden replay can only prove "nothing changed" if nothing was meant to change; mixing fixes into the retrofit would turn every difference into a judgement call.
- Seeded callers (tests, fixtures, games with saved seeds) notice a changed list at once; 3.0 is where such breaks are announced.

## Rejected

- Fixing the list in place in 2.3.0 (what 2.1.0 did to the duplicates): breaks seeded output, the thing the contract exists for.
- Waiting for the next real change before releasing: leaves the new release path unproven until someone needs it.

Related: builds on [../notes/2026-09-28-phase-0-gap-audit.md](../notes/2026-09-28-phase-0-gap-audit.md); see also [../plans/2026-09-28-retrofit-and-2.2.1-release.md](../plans/2026-09-28-retrofit-and-2.2.1-release.md).
