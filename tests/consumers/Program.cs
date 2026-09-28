// A fresh consumer of the packed or published RandomNameGeneratorLibrary, run by run.sh on net10.0 and net48 (from the
// package-modernize template). Calls the public API only; the seeded answers come from the golden recordings (person
// and catalogue star names from tests/Golden/2.2.0.*, the place name from the 2.3.0 place list). Returns 0 only when
// every answer is right, and prints the package version it loaded.

using System;
using System.Linq;
using System.Reflection;
using RandomNameGeneratorLibrary;

public static class Program
{
    public static int Main()
    {
        var checks = new (string What, string Got, string Want)[]
        {
            ("seeded person", new PersonNameGenerator(42).GenerateRandomFirstAndLastName(), "Alisa Streets"),
            ("seeded place", new PlaceNameGenerator(42).GenerateRandomPlaceName(), "Boardman"),
            ("seeded catalogue star", new StarNameGenerator(42).GenerateRandomCatalogStarName(), "HIP 4210"),
            ("Random extension", new Random(5).GenerateRandomFirstAndLastName(), "Abraham Swearngin"),
            ("restored place", PlaceNameGenerator.PlaceNames.Contains("Georgetown") ? "listed" : "missing", "listed"),
        };

        var ok = true;
        foreach (var c in checks)
        {
            if (c.Got != c.Want)
            {
                Console.WriteLine(c.What + ": got '" + c.Got + "', want '" + c.Want + "'");
                ok = false;
            }
        }

        var version = typeof(PersonNameGenerator).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion.Split('+')[0];
        Console.WriteLine("RandomNameGeneratorLibrary " + version);
        Console.WriteLine(ok ? "consumer answers as expected" : "wrong answer");
        return ok ? 0 : 1;
    }
}
