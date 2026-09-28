#!/usr/bin/env bash
# From the package-modernize template (CachingServiceWithAOPSupport 2.0.0). Called by ci.yml (packed package) and
# verify-published.yml (nuget.org). RandomNameGeneratorLibrary has no runtime dependencies, so there is one set, `default`.
# Builds and runs fresh consumers of RandomNameGeneratorLibrary VERSION from SOURCE (a folder of packed nupkgs, or "nuget.org"),
# in each dependency set, on net10.0 and, on Windows, net48. Program.cs beside this script prints what it loaded.
# Each consumer gets its own packages folder, so a cached copy of the same version cannot stand in for SOURCE's.
# Usage: tests/consumers/run.sh VERSION SOURCE
set -euo pipefail

version="$1"
source="$2"
here="$(cd "$(dirname "$0")" && pwd)"
work="${RUNNER_TEMP:-${TMPDIR:-/tmp}}/package-consumers"
rm -rf "$work"
mkdir -p "$work"

if [ "$source" != nuget.org ]; then
  source="$(cd "$source" && pwd)"
  # Git Bash on Windows: dotnet nuget add source refuses /d/a/... and D:/a/...; it takes D:\a\... (skill L-073).
  if command -v cygpath >/dev/null; then source="$(cygpath -w "$source")"; fi
fi

frameworks="net10.0"
case "$(uname -s)" in MINGW* | MSYS* | CYGWIN*) frameworks="net10.0 net48" ;; esac

# No runtime dependencies: one set; the consumer checks seeded answers and prints the version it loaded.
sets="default"

extra_refs() {
  case "$1" in
    default) ;;
  esac
}

expected() {
  case "$1" in
    default) printf '%s\n' 'consumer answers as expected' "RandomNameGeneratorLibrary $version" ;;
  esac
}

for tfm in $frameworks; do
  for combo in $sets; do
    dir="$work/$combo-$tfm"
    mkdir -p "$dir"
    cp "$here/Program.cs" "$dir/Program.cs"
    cat > "$dir/Consumer.csproj" <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>$tfm</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="RandomNameGeneratorLibrary" Version="[$version]" />
    <PackageReference Include="Microsoft.NETFramework.ReferenceAssemblies" Version="1.0.3" PrivateAssets="all" />
    $(extra_refs "$combo")
  </ItemGroup>
</Project>
EOF
    # Empty MSBuild files stop the consumer from inheriting this repository's props when the work folder is inside it.
    printf '<Project>\n</Project>\n' > "$dir/Directory.Build.props"
    printf '<Project>\n</Project>\n' > "$dir/Directory.Build.targets"
    dotnet new nugetconfig -o "$dir" >/dev/null
    if [ "$source" != nuget.org ]; then
      dotnet nuget add source "$source" -n local --configfile "$dir/nuget.config" >/dev/null
    fi
    echo "== $combo on $tfm"
    out=$(NUGET_PACKAGES="$work/packages-$combo-$tfm" dotnet run --project "$dir/Consumer.csproj" -c Release 2>&1) || { echo "$out"; exit 1; }
    echo "$out" | tail -5
    while IFS= read -r want; do
      echo "$out" | grep -qF "$want" || { echo "::error::$combo on $tfm: expected '$want'"; exit 1; }
    done < <(expected "$combo")
  done
done
echo "all consumers answered as expected"
