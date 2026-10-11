# Random Name Generator for Unity

Random person and place names drawn from US census data, and the names of real stars. This is the NuGet package [RandomNameGeneratorLibrary](https://www.nuget.org/packages/RandomNameGeneratorLibrary/) for Unity: `Runtime/RandomNameGeneratorLibrary.dll` is its `netstandard2.0` assembly, byte for byte as nuget.org serves it.

```csharp
using RandomNameGeneratorLibrary;

var people = new PersonNameGenerator(42);       // same seed, same names on every platform
string name = people.GenerateRandomFirstAndLastName();
string place = new PlaceNameGenerator().GenerateRandomPlaceName();
string star = new StarNameGenerator().GenerateRandomStarName();
```

**Web builds:** turn on Player Settings > Publishing Settings > Use Embedded Resources. Unity leaves embedded resources out of Web builds otherwise, so the name lists are missing and every generator throws `InvalidOperationException`. The package's editor script logs an error when a Web build has the setting off. Other platforms, IL2CPP included, need nothing.

One generator per thread. The full guide, the method list and the seeding rules are in the [repository README](https://github.com/m4bwav/DotNetRandomNameGenerator#readme) and the [wiki](https://github.com/m4bwav/DotNetRandomNameGenerator/wiki).
