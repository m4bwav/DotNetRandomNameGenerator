---
title: Unity package on OpenUPM (com.m4bwav.random-name-generator)
kind: note
date: 2026-10-11
verified: 2026-10-11
stale_after: 2027-04-11
tags: [unity, openupm, upm, 2.3.0, release]
summary: "why the Unity package ships the nuget.org DLL instead of source, how it is laid out, the upm/ tag scheme, what was checked in Unity, and how to bring it along after a NuGet release; read before touching Packages/ or the OpenUPM listing"
---

# Unity package on OpenUPM

Asked for on 2026-10-11: make the library submittable to OpenUPM, and submit it. The layout and the submission follow UniverseGenerator's (its `ai-docs/notes/2026-10-04-openupm-submission.md`, listed on OpenUPM since 2026-10-04).

## Why a DLL, not source

The name lists are embedded resources (`Resources.*.stripped`, read with `GetManifestResourceStream`). Unity compiles package source through `.asmdef` files and cannot embed resources, so a source package would need a code change to load the lists another way, which risks the seed promise. The package instead carries `lib/netstandard2.0/RandomNameGeneratorLibrary.dll` and its XML docs from the nuget.org package, byte for byte. 2.3.0's DLL: 996,352 bytes, sha256 `414c5ba0ea4cc711082c8a9e361d7a59607eb95f3494656f93fc70a4797f48f6`. Nothing about the NuGet package changes.

## Layout

`Packages/com.m4bwav.random-name-generator/`: `package.json` (version = the NuGet version it carries, `"unity": "2019.4"`, the first LTS with .NET Standard 2.0, not tested), `README.md`, `LICENSE.md` (a copy of the root LICENSE; OpenUPM packs only the package folder), `Runtime/` with the DLL and XML, and the `.meta` file of every file and folder. The `.meta` files were written by Unity 6000.6.4f1 when it imported the folder as a local package; the DLL's is just the guid, which means default plugin settings (any platform). Registry packages are read-only, and Unity ignores any file in one that has no `.meta`. `.gitattributes` marks the DLL binary and the XML `-text`, so Git never rewrites them.

`tools/UnityPackage/sync.py <version>` downloads the nupkg from nuget.org, copies the two files and sets `package.json`'s version; `--check` exits 1 unless the committed files match nuget.org. CI does not run it: it needs the network, and the files only change on a release.

## Tags

OpenUPM builds from Git tags whose version equals `package.json`'s. The `v*` tags are the NuGet releases and older ones have no Unity package, so the Unity package uses its own tags, `upm/<version>` (OpenUPM's documented form, `gitTagPrefix: upm/` in the package YAML; the prefix is a literal filter and is not stripped before version parsing, and a `upm/` tag needs no `v`). `release.yml` triggers on `v*` only, so a `upm/` tag never starts a NuGet release. Tags are admin-only by ruleset 24095615; Mark's `gh` login is admin.

After a NuGet release: `python tools/UnityPackage/sync.py <version>`, pull request, merge once `ci` is green, tag the merge commit `upm/<version>`, push the tag. OpenUPM picks it up within the hour.

## Checked in Unity (2026-10-11)

A throwaway project in the session scratchpad installed the package folder by `file:` path and logged seeded names: `new PersonNameGenerator(42)`, `PlaceNameGenerator(42)` and `StarNameGenerator(42)` five each, `new Random(7).GenerateRandomPlaceName()` and `GenerateRandomFirstAndLastName()`, and the list sizes (88,799 last names, 16,969 places).

- Unity 6000.6.4f1 Editor (Mono), batchmode: the same lines as `dotnet run` against `RandomNameGeneratorLibrary@2.3.0` from nuget.org (`Alisa Streets|Rhoda Verhey|...`, `Boardman|Sedco Hills|...`, `Delta² Gruis|Sika|...`). No compiler errors or warnings from the package.
- Unity 6000.5.8f1 WebGL player (IL2CPP, managed stripping High), run in headless Chrome (`chrome --headless=new --enable-logging=stderr --virtual-time-budget=120000 --dump-dom http://127.0.0.1:8765/`, console lines in stderr; a 60-second budget was too short): **first build failed at runtime** with `InvalidOperationException: Embedded resource 'RandomNameGeneratorLibrary.Resources.dist.female.first.stripped' was not found`. IL2CPP wrote `Data/Resources/RandomNameGeneratorLibrary.dll-resources.dat` with all seven lists, but the Web packager left it out of `webbuild.data`: Web builds drop embedded resources unless `PlayerSettings.WebGL.useEmbeddedResources` is on (Unity manual, webgl-embeddedresources). Stripping was not the cause. With the setting on, the player printed the same lines as .NET. Other IL2CPP platforms keep the resources.
- So the package has `Editor/WebGLEmbeddedResourcesCheck.cs` (asmdef `RandomNameGenerator.Editor`, Editor only, not auto-referenced): an `IPreprocessBuildWithReport` that logs an error when a Web build has the setting off. It does not change the setting, because that is the user's project, and it does not fail the build. Seen firing in a 6000.5.8f1 Web build; compiles without warnings in 6000.5.8f1 and 6000.6.4f1.

Dead end: copying a project's `Packages/manifest.json` after one Unity version has opened it carries that version's default packages (timeline 6.6.0, tetgen), and an older editor then fails with "Project has invalid dependencies". Start each editor from a manifest with only the package. Long scratchpad paths also break imports inside `com.unity.multiplayer.center`; harmless here.

## Submission

See the log entry of 2026-10-11 for the pull request, the tag and the OpenUPM build.

Related: see also [the wiki note](2026-09-28-github-wiki.md) (the README thread-safety sentence it mentions was reworded here).
