"""Keep the Unity package (OpenUPM) in step with a published NuGet release.

The Unity package ships the netstandard2.0 RandomNameGeneratorLibrary.dll and its XML docs exactly as they are in the
nuget.org package: Unity cannot embed the name lists as resources when it compiles C# source, so the package carries
the published assembly instead. This script copies them from nuget.org and sets package.json's version.

    python tools/UnityPackage/sync.py 2.3.0          # copy the 2.3.0 DLL and XML, set package.json to 2.3.0
    python tools/UnityPackage/sync.py --check        # exit 1 unless the committed files match nuget.org

Standard library only. It never builds the DLL locally: what Unity users get is byte for byte what nuget.org serves.
"""

import argparse
import hashlib
import io
import json
import pathlib
import sys
import urllib.request
import zipfile

ROOT = pathlib.Path(__file__).resolve().parents[2]
PACKAGE = ROOT / "Packages" / "com.m4bwav.random-name-generator"
RUNTIME = PACKAGE / "Runtime"
FILES = ["RandomNameGeneratorLibrary.dll", "RandomNameGeneratorLibrary.xml"]
NUPKG = "https://api.nuget.org/v3-flatcontainer/randomnamegeneratorlibrary/{0}/randomnamegeneratorlibrary.{0}.nupkg"


def published(version):
    with urllib.request.urlopen(NUPKG.format(version.lower())) as response:
        archive = zipfile.ZipFile(io.BytesIO(response.read()))
    return {name: archive.read("lib/netstandard2.0/" + name) for name in FILES}


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("version", nargs="?", help="NuGet version to copy (default: package.json's version)")
    parser.add_argument("--check", action="store_true", help="compare only; write nothing")
    args = parser.parse_args()

    manifest_path = PACKAGE / "package.json"
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    version = args.version or manifest["version"]
    files = published(version)

    if args.check:
        problems = []
        if manifest["version"] != version:
            problems.append("package.json says %s, not %s" % (manifest["version"], version))
        for name, data in files.items():
            local = RUNTIME / name
            if not local.exists() or local.read_bytes() != data:
                problems.append("%s differs from nuget.org %s" % (name, version))
        for problem in problems:
            print("FAIL " + problem)
        if not problems:
            for name, data in files.items():
                print("OK %s %s sha256 %s" % (version, name, hashlib.sha256(data).hexdigest()))
        return 1 if problems else 0

    for name, data in files.items():
        (RUNTIME / name).write_bytes(data)
        print("wrote %s (%d bytes, sha256 %s)" % (name, len(data), hashlib.sha256(data).hexdigest()))
    if manifest["version"] != version:
        text = manifest_path.read_bytes().decode("utf-8")
        old = '"version": "%s"' % manifest["version"]
        if old not in text:
            sys.exit("cannot find %s in package.json" % old)
        manifest_path.write_bytes(text.replace(old, '"version": "%s"' % version, 1).encode("utf-8"))
        print("package.json version %s -> %s" % (manifest["version"], version))
    return 0


if __name__ == "__main__":
    sys.exit(main())
