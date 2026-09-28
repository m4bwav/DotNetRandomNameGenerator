#:package RandomNameGeneratorLibrary@2.3.0
#:property PublishAot=false
// wiki-verify for RandomNameGeneratorLibrary 2.3.0: prints every output the wiki's pages show,
// run against the PUBLISHED package from nuget.org, never the working tree. The repository keeps
// this file as ai-docs/notes/2026-09-28-wiki-verify.cs and its output as
// 2026-09-28-wiki-verify.out.txt (LF line endings) so the next release can run it and diff.
//
// Written 2026-09-28 when the wiki (written by hand earlier that day, no program kept) was
// brought under wikiwright's saved-output rule. Run it from a folder outside any project cone:
//   dotnet run wiki-verify.cs > out.txt
// The PowerShell and F# cases need pwsh (PowerShell 7) and dotnet fsi on the PATH; they run the
// pages' snippets as written against the package in the NuGet cache.
//
// Seeded outputs are exact and identical on every run. Unseeded examples ("for example
// "Prophetstown"") cannot be reproduced; for those the program prints the value only after
// checking it is on the list the method draws from, so the page shows a possible answer.
// This runs on .NET 10 only; the repository's golden tests cover .NET Framework 4.8.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RandomNameGeneratorLibrary;

Console.OutputEncoding = Encoding.UTF8;

var asm = typeof(PersonNameGenerator).Assembly;
Show("installed", $"{asm.GetName().Name} {asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Split('+')[0]}");

var male = PersonNameGenerator.MaleFirstNames;
var female = PersonNameGenerator.FemaleFirstNames;
var last = PersonNameGenerator.LastNames;
var placeList = PlaceNameGenerator.PlaceNames;

// ----- Home and Getting started: unseeded examples, each checked against its list -----
bool IsFirst(string n) => male.Contains(n) || female.Contains(n);
bool IsFull(string n) => n.Split(' ') is [var f, var l] && IsFirst(f) && last.Contains(l);
bool IsCatalog(string n) => n.StartsWith("HD ") && int.TryParse(n[3..], out var hd) && hd >= 1 && hd <= 225300;
Possible("home: unseeded examples", ("Deneen Vanvolkinburg", IsFull), ("Prophetstown", placeList.Contains), ("Alpha Muscae", StarNameGenerator.StarNames.Contains));
Possible("getting started: the first call, unseeded", ("Jon Rohl", IsFull), ("Hildegard", female.Contains), ("Benny", male.Contains),
    ("Drouillard", last.Contains), ("Prophetstown", placeList.Contains), ("26 Vulpeculae", StarNameGenerator.StarNames.Contains),
    ("Ankaa", StarNameGenerator.ProperStarNames.Contains), ("HD 107420", IsCatalog));

// ----- seeded examples: exact -----
Show("getting started: seed 42, full name (also Recipes, seeded test data)", new PersonNameGenerator(42).GenerateRandomFirstAndLastName());
Show("getting started: seed 42, place", new PlaceNameGenerator(42).GenerateRandomPlaceName());
var r42 = new Random(42);
Show("getting started: extension methods on one Random(42)", string.Join("\n", r42.GenerateRandomFemaleFirstName(), r42.GenerateRandomPlaceName(), r42.GenerateRandomStarName()));
var a = new PersonNameGenerator(42);
var b = new PersonNameGenerator(new Random(42));
var c = new Random(42);
Show("reproducible names: three ways to seed", string.Join("\n", a.GenerateRandomFemaleFirstName(), b.GenerateRandomFemaleFirstName(), c.GenerateRandomFemaleFirstName()));
var twoCalls = new PersonNameGenerator(42);
Show("recipes: first then last equals first-and-last (seed 42)", twoCalls.GenerateRandomFirstName() + " " + twoCalls.GenerateRandomLastName());

// ----- Recipes -----
var people1 = new PersonNameGenerator(1);
var lastNames = new HashSet<string>();
while (lastNames.Count < 5)
{
    lastNames.Add(people1.GenerateRandomLastName());
}

Show("recipes: distinct names", string.Join(", ", lastNames));

Show("recipes: one Random for everything", Captured(() =>
{
    var random = new Random(2026);
    var people = new PersonNameGenerator(random);
    var places = new PlaceNameGenerator(random);
    for (var i = 0; i < 3; i++)
    {
        Console.WriteLine($"{people.GenerateRandomFirstAndLastName()} from {places.GenerateRandomPlaceName()}");
    }
}));
var space = new Random(2026);
Show("recipes: the same seed with a StarNameGenerator", $"{new PersonNameGenerator(space).GenerateRandomFirstAndLastName()} of {new StarNameGenerator(space).GenerateRandomStarName()}");

Show("recipes: usernames", Captured(() =>
{
    var people = new PersonNameGenerator(99);
    for (var i = 0; i < 3; i++)
    {
        var first = people.GenerateRandomFirstName();
        var surname = people.GenerateRandomLastName();
        Console.WriteLine($"{first} {surname} -> {first.ToLowerInvariant()}.{surname.ToLowerInvariant()}@example.com");
    }
}));

var mc = PersonNameGenerator.LastNames.Where(n => n.StartsWith("Mc")).ToList();
Show("recipes: picking from the lists (the Mc names)", $"{mc.Count:N0} names");
Show("recipes: picking from the lists (three of them)", Captured(() =>
{
    var random = new Random(3);
    Console.WriteLine(string.Join(", ", Enumerable.Range(0, 3).Select(_ => mc[random.Next(mc.Count)])));
}));

// ----- facts the pages state in prose -----
string Longest(IEnumerable<string> names) => names.OrderByDescending(n => n.Length).First();
Show("sizes: male, female, last, places, stars, proper, designated, catalogue",
    $"{male.Count:N0} {female.Count:N0} {last.Count:N0} {placeList.Count:N0} {StarNameGenerator.StarNames.Count:N0} {StarNameGenerator.ProperStarNames.Count:N0} {StarNameGenerator.DesignatedStarNames.Count:N0} {StarNameGenerator.CatalogStarNameCount:N0}");
Show("first entries: male, female, last, place, last place, proper, designated",
    string.Join(" | ", male[0], female[0], last[0], placeList[0], placeList[^1], StarNameGenerator.ProperStarNames[0], StarNameGenerator.DesignatedStarNames[0]));
Show("places with a space, places with a non-ASCII letter", $"{placeList.Count(p => p.Contains(' ')):N0} {placeList.Count(p => p.Any(ch => ch > 127))}");
Show("first names of four letters or fewer: female, male; on both lists",
    $"{female.Count(n => n.Length <= 4)} {male.Count(n => n.Length <= 4)} {male.Intersect(female).Count()}");
Show("longest last name, longest place name", Longest(last) + " | " + Longest(placeList));
Show("names of the longest length: last names (length, count, first three), place names",
    $"{Longest(last).Length} characters, {last.Count(n => n.Length == Longest(last).Length)}: {string.Join(", ", last.Where(n => n.Length == Longest(last).Length).Take(3))}\n" +
    $"{Longest(placeList).Length} characters: {string.Join(" | ", placeList.Where(n => n.Length == Longest(placeList).Length))}");
var every = male.Concat(female).Concat(last).Concat(placeList).Concat(StarNameGenerator.StarNames).ToList();
Show("places with a comma", string.Join(" | ", placeList.Where(p => p.Contains(','))));
Show("names with an apostrophe (places), with a double quotation mark (all lists)", $"{placeList.Count(p => p.Contains('\''))} {every.Count(n => n.Contains('"'))}");
Show("non-ASCII examples on the lists", string.Join(" | ", new[] { "Mayagüez zona urbana", "Boötis", "Rosalíadecastro", "Delta² Gruis" }
    .Select(n => n + (placeList.Contains(n) || StarNameGenerator.StarNames.Any(s => s.Contains(n)) ? " found" : " MISSING"))));
Show("person names are ASCII letters only", every.Take(male.Count + female.Count + last.Count).All(n => n.All(char.IsAsciiLetter)).ToString());

// ----- API reference: counts and errors -----
Show("zero gives an empty sequence", new PersonNameGenerator(1).GenerateMultipleLastNames(0).Count().ToString());
Show("negative count", Catch(() => new PersonNameGenerator(1).GenerateMultipleLastNames(-1)));
Show("negative count, places", Catch(() => new PlaceNameGenerator(1).GenerateMultiplePlaceNames(-1)));
Show("null Random", Catch(() => new PersonNameGenerator((Random)null!)));

// ----- the pages' F# and PowerShell snippets, run as written -----
var fsx = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "rng-wiki-verify.fsx");
System.IO.File.WriteAllText(fsx, """
    #r "nuget: RandomNameGeneratorLibrary, 2.3.0"
    open System
    open RandomNameGeneratorLibrary

    let people = PersonNameGenerator(42)
    printfn "%s" (people.GenerateRandomFirstAndLastName())   // Alisa Streets
    printfn "%s" (Random(42).GenerateRandomPlaceName())      // Boardman
    """);
Show("getting started: F#", Run("dotnet", "fsi", "--quiet", fsx));
var ps = Run("pwsh", "-NoProfile", "-NonInteractive", "-Command", """
    Add-Type -Path "$env:USERPROFILE/.nuget/packages/randomnamegeneratorlibrary/2.3.0/lib/netstandard2.0/RandomNameGeneratorLibrary.dll"
    [RandomNameGeneratorLibrary.PersonNameGenerator]::new().GenerateRandomFirstAndLastName()
    [RandomNameGeneratorLibrary.PlaceNameGenerator]::new(42).GenerateRandomPlaceName()   # Boardman
    [RandomNameGeneratorLibrary.PersonNameGenerator]::LastNames.Count                     # 88799
    """).Split('\n');
// The first line is unseeded: print whether it is a possible answer, not the name, so the saved
// output stays the same on every run.
Show("getting started: PowerShell", string.Join("\n", ps.Select((line, i) => i == 0 ? (IsFull(line) ? "<an unseeded full name from the lists>" : "NOT ON THE LISTS: " + line) : line)));

static void Show(string label, object? value)
{
    Console.WriteLine($"## {label}");
    Console.WriteLine(value?.ToString() ?? "<null>");
    Console.WriteLine();
}

// Prints each value only when its check passes, one per line, so a page's example is shown to be
// a possible answer of an unseeded call.
static void Possible(string label, params (string Value, Func<string, bool> OnList)[] cases)
{
    Show(label + " (each checked against the list it comes from)",
        string.Join("\n", cases.Select(c => c.OnList(c.Value) ? c.Value : "NOT ON THE LIST: " + c.Value)));
}

// What a page's code prints with Console.WriteLine, line for line.
static string Captured(Action action)
{
    var original = Console.Out;
    var writer = new System.IO.StringWriter { NewLine = "\n" };
    Console.SetOut(writer);
    try
    {
        action();
    }
    finally
    {
        Console.SetOut(original);
    }

    return writer.ToString().TrimEnd('\n');
}

// Runs a page's snippet in another host and returns what it printed, LF line endings.
static string Run(string file, params string[] args)
{
    var info = new System.Diagnostics.ProcessStartInfo(file) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
    foreach (var arg in args)
    {
        info.ArgumentList.Add(arg);
    }

    using var p = System.Diagnostics.Process.Start(info)!;
    var stdout = p.StandardOutput.ReadToEnd();
    var stderr = p.StandardError.ReadToEnd();
    p.WaitForExit();
    var text = stdout + (stderr.Length > 0 ? "--- stderr\n" + stderr : "") + (p.ExitCode != 0 ? "--- exit " + p.ExitCode : "");
    return text.Replace("\r\n", "\n").TrimEnd();
}

static string Catch(Func<object?> action)
{
    try
    {
        return "returned: " + (action()?.ToString() ?? "<null>");
    }
    catch (Exception e)
    {
        return $"{e.GetType().Name}: {e.Message}";
    }
}
