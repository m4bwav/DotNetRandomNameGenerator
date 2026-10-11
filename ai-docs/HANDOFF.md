# Handoff

## Current state
2.3.0 is released and verified on nuget.org; the retrofit is complete (history in log.md and plans/2026-09-28-retrofit-and-2.3.0-release.md). 2026-10-11: the Unity package for OpenUPM was built and checked, PR #22 ([notes/2026-10-11-openupm-unity-package.md](notes/2026-10-11-openupm-unity-package.md)). The agent's merge was refused by the auto-mode classifier, so the PR waits for Mark.

## Next single action
After PR #22 is merged, with `ci` green on master:

1. `git fetch origin && python tools/UnityPackage/sync.py --check` on origin/master, then `git tag -a upm/2.3.0 -m "Unity package 2.3.0 (OpenUPM)" origin/master && git push origin upm/2.3.0`. `release.yml` ignores it (it triggers on `v*`).
2. Submit to OpenUPM the way UniverseGenerator was (fork m4bwav/openupm, synced with `gh repo sync m4bwav/openupm -b master`): branch, add `data/packages/com.m4bwav.random-name-generator.yml`, open a pull request against openupm/openupm. Mergify merges a single data file once validation passes. The YAML (replace SHA with the master commit that holds `.github/images/openupm-cover.jpg`):

```yaml
name: com.m4bwav.random-name-generator
displayName: Random Name Generator
description: >-
  Random person and place names drawn from US census data, and the names of
  real stars. Seed a generator and you get the same names on every platform.
repoUrl: https://github.com/m4bwav/DotNetRandomNameGenerator
trackingMode: git
parentRepoUrl: null
licenseSpdxId: MIT
licenseName: MIT License
image: >-
  https://raw.githubusercontent.com/m4bwav/DotNetRandomNameGenerator/SHA/.github/images/openupm-cover.jpg
topics:
  - procedural-generation
hunter: m4bwav
gitTagPrefix: upm/
gitTagIgnore: ''
minVersion: ''
readme: master:README.md
```

3. After the build (15 to 30 minutes), check https://openupm.com/packages/com.m4bwav.random-name-generator/ and install it from `https://package.openupm.com` in a throwaway Unity project (the note has the RngCheck script and the expected lines).

## Standing work
Dependabot pull requests (seven-day cooldown); Mark's nuget.org deprecations of 1.1.0 (Critical bugs) and 1.0.5.1, 1.1.1, 1.2.0, 1.2.1 (Legacy, alternate 2.x); the next major (3.0) removes CensusListStripper and FileCompressor. After each NuGet release, bring the Unity package along (AGENTS.md, release ritual).
