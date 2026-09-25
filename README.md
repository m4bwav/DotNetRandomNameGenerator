DotNetRandomNameGenerator
=========================

Generates random people and place names drawn from freely available US census data.

[![NuGet](https://img.shields.io/nuget/v/RandomNameGeneratorLibrary.svg)](https://www.nuget.org/packages/RandomNameGeneratorLibrary/) [![CI](https://github.com/m4bwav/DotNetRandomNameGenerator/actions/workflows/ci.yml/badge.svg)](https://github.com/m4bwav/DotNetRandomNameGenerator/actions/workflows/ci.yml)

```
dotnet add package RandomNameGeneratorLibrary
```

Targets `netstandard2.0` (any .NET Framework 4.6.2+, .NET Core, Mono, Unity) and `net10.0`, with no dependencies. Version 2.1 keeps the 1.2.2 / 2.0 API and adds seed constructors and read-only access to the lists; see `CHANGELOG.md`. Try it live at https://www.markdavidrogers.com/tools (random names) or through the site's MCP server tool `random_name`.

## What the names look like

Eight of each, straight from the library. Each row comes from a fresh generator built with `new Random(20260924)` and one `GenerateMultiple...(8)` call, so you can reproduce them:

| Kind | Examples |
| --- | --- |
| First and last name | Jon Rohl, Deneen Vanvolkinburg, Alona Glordano, Norris Veltz, Wes Tomopoulos, Peter Melito, Stephanie Niceswander, Erich Dunnagan |
| Female first name | Hildegard, Laurel, Gertie, Jodee, Deneen, Margaretta, Margorie, Alona |
| Male first name | Benny, Jon, Lowell, Quincy, Tyson, Winfred, Aurelio, Alonso |
| Last name | Drouillard, Batres, Rohl, Tabar, Babilonia, Vanvolkinburg, Vendrick, Absalon |
| Place name | Phillips, Nebo Center, Smithton, Mindenmines, Popponesset, Lyndhurst, Little Silver, Vega Baja zona urbana |

The lists have their frequencies stripped, so every entry on a list is equally likely: common names and rare ones sit side by side, and a few first names appear on both the male and female lists because the census recorded them that way.

## Usage

Create a `PersonNameGenerator` or `PlaceNameGenerator` in namespace `RandomNameGeneratorLibrary` and call a method such as `GenerateRandomFirstAndLastName()`. Every method name says literally what it does.

Generating a person name:
```C#
var personGenerator = new PersonNameGenerator();
var name = personGenerator.GenerateRandomFirstAndLastName();

Console.WriteLine(name); // "{first} {last}", for example "Mark Rogers"
```

Generating a place name:
```C#
var placeGenerator = new PlaceNameGenerator();
var name = placeGenerator.GenerateRandomPlaceName();

Console.WriteLine(name); // for example "Hoboken"
```

Male and female first names can be generated with or without a last name:
```C#
var personGenerator = new PersonNameGenerator();
var name = personGenerator.GenerateRandomFemaleFirstName();

Console.WriteLine(name); // for example "Jane"
```

More than one name can be generated at a time. There are methods for each gender and for last names alone, and each takes the number of names to produce:
```C#
var personGenerator = new PersonNameGenerator();
var names = personGenerator.GenerateMultipleFemaleFirstAndLastNames(5);

Console.WriteLine(names.First()); // for example "Rose Jones"
```

For reproducible names, pass a seed or your own `Random`:
```C#
var personGenerator = new PersonNameGenerator(42);           // same seed, same names
var another = new PersonNameGenerator(new Random(42));       // equivalent
var name = personGenerator.GenerateRandomFemaleFirstName();

Console.WriteLine(name); // for example "Lindsay"
```

The same functionality is available as extension methods on `Random`:
```C#
var randomObj = new Random(42);
var name = randomObj.GenerateRandomFemaleFirstName();
var place = randomObj.GenerateRandomPlaceName();
```

The lists themselves are exposed read-only, so you can count them or pick your own way:
```C#
Console.WriteLine(PersonNameGenerator.LastNames.Count);     // 88799
Console.WriteLine(PlaceNameGenerator.PlaceNames[0]);        // first entry of the place list
```

`IPersonNameGenerator` and `IPlaceNameGenerator` cover the generating methods, for dependency injection and for mocking in tests.

### Thread safety

A generator made with the default constructor is safe to share between threads (it uses `Random.Shared` on .NET 6+ and a well-seeded `Random` per generator on .NET Framework). A generator made around your own `Random`, including the extension methods, is only as thread-safe as that `Random`, which is not at all: use one generator per thread. The lists are parsed once per process, on first use, and are safe to read from any thread.

### Unity

The `netstandard2.0` build works in Unity: no dependencies, no runtime reflection beyond reading its own embedded resources, and no runtime regular expressions. Copy the DLL from the package's `lib/netstandard2.0` folder into `Assets/Plugins`, or use a NuGet-for-Unity tool.

## Data

| List | Entries | Source |
| --- | --- | --- |
| Male first names | 1,219 | 1990 US Census name frequency file `dist.male.first` |
| Female first names | 4,275 | 1990 US Census name frequency file `dist.female.first` |
| Last names | 88,799 | 1990 US Census name frequency file `dist.all.last` |
| Place names | 16,873 | Census 2000 places file `places2k.txt`, one entry per distinct name |

Person names are ASCII and Title case (`Mark`, `Rogers`). The place list drops the state and the classification word (`city`, `town`, `CDP`, ...), keeps accents on Puerto Rico names (`Bayamón zona urbana`, `Mayagüez zona urbana`) and lists a name once no matter how many states have it, so `Franklin` is exactly as likely as `Popponesset`.

`GenerateRandomFirstName` picks male or female with equal probability first, then a name from that list, so any one male name is about 3.5 times likelier than any one female name. Use the gendered methods if you want uniform odds within a list.

The lists are embedded resources (`RandomNameGeneratorLibrary/Resources.*.stripped`, LF line endings, UTF-8). They were produced by the `tools/CensusTools` console project in this repository, which reads the Census files as Latin-1 and removes duplicates; it is not part of the package. The `CensusListStripper` and `FileCompressor` classes still in the library are the original one-off tools, marked obsolete, and will be removed in 3.0.

## Building and releasing

```
dotnet test
dotnet pack RandomNameGeneratorLibrary -c Release -o artifacts
```

CI (`.github/workflows/ci.yml`) restores in locked mode, builds, checks formatting, runs the tests on Linux (net10.0) and Windows (net10.0 and net48, which exercises the netstandard2.0 build), collects coverage and packs on every push. To publish: add the entry to `CHANGELOG.md`, bump `<Version>` in `RandomNameGeneratorLibrary.csproj`, tag the commit `v<version>` and push the tag. The `publish` job checks that the tag matches the version, signs in to nuget.org with Trusted Publishing (GitHub OIDC, no stored API key; a `NUGET_USER` secret holding the nuget.org profile name lives in the `nuget` environment), pushes the package and creates a GitHub Release with the `.nupkg` and `.snupkg` attached. Package validation compares the public API against 2.0.1 at pack time, so an accidental breaking change fails the build.

## License

MIT, see `LICENSE`.
