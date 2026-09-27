DotNetRandomNameGenerator
=========================

Generates random people and place names drawn from freely available US census data, and the names of real stars.

[![NuGet](https://img.shields.io/nuget/v/RandomNameGeneratorLibrary.svg)](https://www.nuget.org/packages/RandomNameGeneratorLibrary/) [![CI](https://github.com/m4bwav/DotNetRandomNameGenerator/actions/workflows/ci.yml/badge.svg)](https://github.com/m4bwav/DotNetRandomNameGenerator/actions/workflows/ci.yml)

```
dotnet add package RandomNameGeneratorLibrary
```

Targets `netstandard2.0` (any .NET Framework 4.6.2+, .NET Core, Mono, Unity) and `net10.0`, with no dependencies. Version 2.2 keeps the 1.2.2 / 2.0 API, and adds star names (2.2) and seed constructors and read-only access to the lists (2.1); see `CHANGELOG.md`. Try it live at https://www.markdavidrogers.com/tools (random names) or through the site's MCP server tool `random_name`.

## What the names look like

Eight of each, straight from the library. Each row comes from a fresh generator built with `new Random(20260924)` and one `GenerateMultiple...(8)` call, so you can reproduce them (the proper and catalogue star rows come from eight single calls on one such generator):

| Kind | Examples |
| --- | --- |
| First and last name | Jon Rohl, Deneen Vanvolkinburg, Alona Glordano, Norris Veltz, Wes Tomopoulos, Peter Melito, Stephanie Niceswander, Erich Dunnagan |
| Female first name | Hildegard, Laurel, Gertie, Jodee, Deneen, Margaretta, Margorie, Alona |
| Male first name | Benny, Jon, Lowell, Quincy, Tyson, Winfred, Aurelio, Alonso |
| Last name | Drouillard, Batres, Rohl, Tabar, Babilonia, Vanvolkinburg, Vendrick, Absalon |
| Place name | Phillips, Nebo Center, Smithton, Mindenmines, Popponesset, Lyndhurst, Little Silver, Vega Baja zona urbana |
| Star name | 26 Vulpeculae, Revati, 28 Cygni, 78 Ursae Majoris, 55 Sagittarii, Alpha Muscae, Alpha Monocerotis, Zeta¹ Scorpii |
| Proper star name | Elkurud, Ankaa, Enduri Senggu, Meissa, Keid, Muning, Muning, Áldu |
| Catalogue star name | HD 107420, HD 44143, HD 109927, HD 189997, HD 160253, HD 207433, HD 207333, HIP 118218 |

The lists have their frequencies stripped, so every entry on a list is equally likely: common names and rare ones sit side by side, and a few first names appear on both the male and female lists because the census recorded them that way.

## Usage

Create a `PersonNameGenerator`, `PlaceNameGenerator` or `StarNameGenerator` in namespace `RandomNameGeneratorLibrary` and call a method such as `GenerateRandomFirstAndLastName()`. Every method name says literally what it does.

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

Generating a star name:
```C#
var starGenerator = new StarNameGenerator();

Console.WriteLine(starGenerator.GenerateRandomStarName());        // an IAU name or a designation, for example "Tau Ceti"
Console.WriteLine(starGenerator.GenerateRandomProperStarName());  // IAU proper names only, for example "Betelgeuse"
Console.WriteLine(starGenerator.GenerateRandomCatalogStarName()); // any of 343,518 catalogued stars, for example "HD 209458"
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
var star = randomObj.GenerateRandomStarName();
```

The lists themselves are exposed read-only, so you can count them or pick your own way:
```C#
Console.WriteLine(PersonNameGenerator.LastNames.Count);     // 88799
Console.WriteLine(PlaceNameGenerator.PlaceNames[0]);        // first entry of the place list
```

`IPersonNameGenerator`, `IPlaceNameGenerator` and `IStarNameGenerator` cover the generating methods, for dependency injection and for mocking in tests.

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
| Proper star names | 640 | IAU Catalog of Star Names, IAU Working Group on Star Names ([exopla.net](https://exopla.net/star-names/modern-iau-star-names/)), fetched 2026-09-27 |
| Designated star names | 3,076 | Bayer and Flamsteed names in the Yale Bright Star Catalogue, 5th ed. (Hoffleit and Warren 1991, VizieR `V/50`) |
| Catalogue star names | 343,518 | Henry Draper catalogue HD 1 to HD 225300 and the 118,218 stars of the Hipparcos catalogue (ESA 1997, VizieR `I/239`) |

Person names are ASCII and Title case (`Mark`, `Rogers`). The place list drops the state and the classification word (`city`, `town`, `CDP`, ...), keeps accents on Puerto Rico names (`Bayamón zona urbana`, `Mayagüez zona urbana`) and lists a name once no matter how many states have it, so `Franklin` is exactly as likely as `Popponesset`.

`GenerateRandomFirstName` picks male or female with equal probability first, then a name from that list, so any one male name is about 3.5 times likelier than any one female name. Use the gendered methods if you want uniform odds within a list.

### Stars

No package can hold every known star: Gaia alone has catalogued about 1.8 billion. The star lists instead aim for every star a person would know or could look up:

- **Proper names** are the whole IAU-approved list, spelled as the IAU spells them, diacritics included (`Rosalíadecastro`, `Chasoň`).
- **Designations** cover the Yale Bright Star Catalogue, which holds every star visible to the naked eye. Each is written out in full (`Gamma Piscis Austrini`, `Kappa¹ Sculptoris`, with Unicode superscript digits); a star with a Greek letter is listed by it rather than by its Flamsteed number, and a designation that is also an IAU name is listed once. `GenerateRandomStarName` draws from these two lists together (3,716 names), so a well-known name comes up one draw in six.
- **Catalogue designations** are drawn from 343,518 real stars without storing them: Henry Draper numbers run from 1 to 225,300 without gaps, and the Hipparcos numbers are kept as the 2,198 numbers the catalogue skips. The star data adds about 61 KB to the assembly and about 45 KB to the package.

`tools/StarLists/build_star_lists.py` downloads the three sources and rebuilds the star resources. IAU material is published under Creative Commons Attribution; please keep the credit above if you redistribute the lists.

The lists are embedded resources (`RandomNameGeneratorLibrary/Resources.*.stripped`, LF line endings, UTF-8). They were produced by the `tools/CensusTools` console project in this repository, which reads the Census files as Latin-1 and removes duplicates; it is not part of the package. The `CensusListStripper` and `FileCompressor` classes still in the library are the original one-off tools, marked obsolete, and will be removed in 3.0.

## Building and releasing

```
dotnet test
dotnet pack RandomNameGeneratorLibrary -c Release -o artifacts
```

CI (`.github/workflows/ci.yml`) restores in locked mode, builds, checks formatting, runs the tests on Linux (net10.0) and Windows (net10.0 and net48, which exercises the netstandard2.0 build), collects coverage and packs on every push. To publish: add the entry to `CHANGELOG.md`, bump `<Version>` in `RandomNameGeneratorLibrary.csproj`, tag the commit `v<version>` and push the tag. The `publish` job checks that the tag matches the version, signs in to nuget.org with Trusted Publishing (GitHub OIDC, no stored API key; a `NUGET_USER` secret holding the nuget.org profile name lives in the `nuget` environment), pushes the package and creates a GitHub Release with the `.nupkg` and `.snupkg` attached. Package validation compares the public API against 2.0.1 at pack time, so an accidental breaking change fails the build.

## License

MIT, see `LICENSE`.
