---
title: FantasyNameGenerator, a new public type of word-grammar names
kind: decision
status: proposed
date: 2026-11-27
verified: 2026-11-27
stale_after: never
tags: [api, fantasy, grammars, minor-release, public-api]
summary: "why FantasyNameGenerator, IFantasyNameGenerator and Landform were added as new types instead of new members on the existing generators, how they are seeded, and how the public API test allows them; read before adding public API"
---

# FantasyNameGenerator

## Context

A request for fantasy names (places, landform places, factions, war-bands, trade surnames, bynames, keeper epithets) built from word grammars, with the library's own word lists. Technique ideas came from Finch's Tome of Adventure Design, Points of Light, Gygax's Living Fantasy and World Builder, and the Judges Guild Guidebook to Shops; no list is copied from them.

## Decision (awaiting the maintainer's review in the PR)

- New types only (`FantasyNameGenerator : BaseNameGenerator`, `IFantasyNameGenerator`, `Landform`); no existing member, list or `Random` call order changes, so every golden recording still replays.
- Seeding follows the other generators: `()`, `(Random)`, `(int seed)`. Bynames draw the first name from the census lists through a `PersonNameGenerator` sharing the same `Random`.
- Word lists live in code, not as embedded resources: they are short and hand written, not generated data.
- The seeded output of the new type is promised only within a version (stated in its remarks), so word lists can grow in minor releases without a golden decision.
- `PublicApiBehavior.NoPublicMemberWasAddedWithoutAPlanDecision` now also accepts lines in `RandomNameGeneratorUnitTests/PublicApi-additions.txt`; each block there must point at a decision entry. The 2.2.0 baseline file stays untouched.

## Not done here

- No version bump, no release, no Unity sync. A minor release (2.4.0) would carry it; bump `PackageValidationBaselineVersion` after release as usual.
