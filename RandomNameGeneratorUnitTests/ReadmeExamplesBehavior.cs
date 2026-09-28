using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RandomNameGeneratorLibrary;
using Xunit;

namespace RandomNameGeneratorUnitTests
{
    // The README's "What the names look like" rows are seeded, so they must reproduce: each from a fresh generator built
    // with new Random(20260924) and one GenerateMultiple...(8) call, the proper and catalogue star rows from eight single
    // calls on one generator. A change to a list or to how the generators draw shows up here as a stale README.
    public class ReadmeExamplesBehavior
    {
        private const int Seed = 20260924;

        public static IEnumerable<object[]> Rows()
        {
            yield return new object[] { "First and last name" };
            yield return new object[] { "Female first name" };
            yield return new object[] { "Male first name" };
            yield return new object[] { "Last name" };
            yield return new object[] { "Place name" };
            yield return new object[] { "Star name" };
            yield return new object[] { "Proper star name" };
            yield return new object[] { "Catalogue star name" };
        }

        [Theory]
        [MemberData(nameof(Rows))]
        public void ReadmeExampleRowReproduces(string kind)
        {
            Assert.Equal(Generate(kind), ReadmeRow(kind));
        }

        private static IEnumerable<string> Generate(string kind)
        {
            switch (kind)
            {
                case "First and last name": return new PersonNameGenerator(new Random(Seed)).GenerateMultipleFirstAndLastNames(8);
                case "Female first name": return new PersonNameGenerator(new Random(Seed)).GenerateMultipleFemaleFirstNames(8);
                case "Male first name": return new PersonNameGenerator(new Random(Seed)).GenerateMultipleMaleFirstNames(8);
                case "Last name": return new PersonNameGenerator(new Random(Seed)).GenerateMultipleLastNames(8);
                case "Place name": return new PlaceNameGenerator(new Random(Seed)).GenerateMultiplePlaceNames(8);
                case "Star name": return new StarNameGenerator(new Random(Seed)).GenerateMultipleStarNames(8);
                case "Proper star name":
                    var proper = new StarNameGenerator(new Random(Seed));
                    return Enumerable.Range(0, 8).Select(_ => proper.GenerateRandomProperStarName()).ToList();
                case "Catalogue star name":
                    var catalogue = new StarNameGenerator(new Random(Seed));
                    return Enumerable.Range(0, 8).Select(_ => catalogue.GenerateRandomCatalogStarName()).ToList();
                default: throw new ArgumentOutOfRangeException(nameof(kind), kind, "not a README row");
            }
        }

        private static List<string> ReadmeRow(string kind)
        {
            var prefix = "| " + kind + " | ";
            var line = File.ReadAllLines(FindReadme()).Single(l => l.StartsWith(prefix, StringComparison.Ordinal));
            return line.Substring(prefix.Length).TrimEnd(' ', '|').Split(new[] { ", " }, StringSplitOptions.None).ToList();
        }

        private static string FindReadme()
        {
            for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            {
                var candidate = Path.Combine(dir.FullName, "README.md");
                if (File.Exists(candidate) && File.Exists(Path.Combine(dir.FullName, "DotNetRandomNameGenerator.slnx")))
                {
                    return candidate;
                }
            }

            throw new FileNotFoundException("README.md next to DotNetRandomNameGenerator.slnx was not found above " + AppContext.BaseDirectory);
        }
    }
}
