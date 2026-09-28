# Security policy

## Reporting a vulnerability

Report it privately through GitHub: open the repository's **Security** tab and choose **Report a vulnerability**. Please do not open a public issue for a security problem.

A confirmed problem is fixed in a new release, and the advisory is published once the fix is on nuget.org. Affected versions are then marked deprecated on nuget.org with the fixed version as the alternate.

## Supported versions

Only the latest major version (2.x) gets security fixes.

## What this package is not

RandomNameGeneratorLibrary makes no network calls and reads only its own embedded lists. Its names come from `System.Random` and are not secret or unpredictable: never use them for passwords, tokens or anything an attacker must not guess. The obsolete `CensusListStripper` and `FileCompressor` classes read and write whatever file paths the caller passes; they are build-time tools, kept only for compatibility until 3.0.
