using System;
using System.Collections.Generic;
using System.Linq;
using RandomNameGeneratorLibrary;
using Xunit;

namespace RandomNameGeneratorUnitTests
{
    public class StarNameBehavior
    {
        [Fact]
        public void ShouldGenerateRandomName()
        {
            var name = new StarNameGenerator().GenerateRandomStarName();

            Assert.False(string.IsNullOrWhiteSpace(name));
        }

        [Fact]
        public void SeedConstructorMatchesSeededRandomConstructor()
        {
            var fromSeed = new StarNameGenerator(7).GenerateMultipleStarNames(5);
            var fromRandom = new StarNameGenerator(new Random(7)).GenerateMultipleStarNames(5);

            Assert.Equal(fromRandom, fromSeed);
        }

        [Fact]
        public void GenerateMultipleStarNamesReturnsTheRequestedCountFromTheList()
        {
            var stars = new HashSet<string>(StarNameGenerator.StarNames);
            var names = new StarNameGenerator(9).GenerateMultipleStarNames(50).ToList();

            Assert.Equal(50, names.Count);
            Assert.All(names, name => Assert.Contains(name, stars));
        }

        [Fact]
        public void ProperStarNamesComeFromTheIauList()
        {
            var generator = new StarNameGenerator(3);

            for (var i = 0; i < 50; i++)
                Assert.Contains(generator.GenerateRandomProperStarName(), StarNameGenerator.ProperStarNames);
        }

        [Fact]
        public void StarNamesAreProperNamesFollowedByDesignations()
        {
            Assert.Equal(StarNameGenerator.ProperStarNames.Concat(StarNameGenerator.DesignatedStarNames), StarNameGenerator.StarNames);
        }

        [Fact]
        public void WellKnownStarsArePresent()
        {
            Assert.Contains("Sirius", StarNameGenerator.ProperStarNames);
            Assert.Contains("Betelgeuse", StarNameGenerator.ProperStarNames);
            Assert.Contains("Polaris", StarNameGenerator.ProperStarNames);
            Assert.Contains("Tau Ceti", StarNameGenerator.DesignatedStarNames);
            Assert.Contains("61 Cygni", StarNameGenerator.DesignatedStarNames);
            Assert.Contains("Alpha¹ Centauri", StarNameGenerator.DesignatedStarNames);
            Assert.Contains("Epsilon Eridani", StarNameGenerator.DesignatedStarNames);
        }

        [Fact]
        public void CatalogStarNamesAreHenryDraperOrHipparcosDesignations()
        {
            var generator = new StarNameGenerator(5);
            var names = Enumerable.Range(0, 2000).Select(_ => generator.GenerateRandomCatalogStarName()).ToList();

            Assert.All(names, name =>
            {
                var parts = name.Split(' ');
                Assert.Equal(2, parts.Length);
                Assert.Contains(parts[0], new[] { "HD", "HIP" });
                var number = int.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                Assert.InRange(number, 1, parts[0] == "HD" ? 225300 : 120416);
            });
            Assert.Contains(names, name => name.StartsWith("HD ", StringComparison.Ordinal));
            Assert.Contains(names, name => name.StartsWith("HIP ", StringComparison.Ordinal));
        }

        [Fact]
        public void CatalogCoversEveryHenryDraperAndHipparcosStar()
        {
            Assert.Equal(225300 + 118218, StarNameGenerator.CatalogStarNameCount);
        }

        [Fact]
        public void CatalogSkipsHipparcosNumbersThatDoNotExist()
        {
            // HIP 672 and HIP 1569 are gaps in the Hipparcos catalogue; a seeded sweep must never produce them.
            var generator = new StarNameGenerator(11);
            var names = new HashSet<string>(Enumerable.Range(0, 20000).Select(_ => generator.GenerateRandomCatalogStarName()));

            Assert.DoesNotContain("HIP 672", names);
            Assert.DoesNotContain("HIP 1569", names);
        }

        [Fact]
        public void ZeroCountReturnsEmpty()
        {
            Assert.Empty(new StarNameGenerator(1).GenerateMultipleStarNames(0));
        }

        [Fact]
        public void NegativeCountThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new StarNameGenerator(1).GenerateMultipleStarNames(-1));
        }

        [Fact]
        public void NullRandomThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new StarNameGenerator(null!));
        }

        [Fact]
        public void DefaultGeneratorsCreatedInATightLoopDoNotRepeatEachOther()
        {
            var names = new List<string>();
            for (var i = 0; i < 20; i++)
                names.Add(new StarNameGenerator().GenerateRandomStarName());

            Assert.True(names.Distinct().Count() > 1, "every default generator produced the same name");
        }

        [Fact]
        public void ExtensionsMatchTheGenerator()
        {
            Assert.Equal(new StarNameGenerator(11).GenerateRandomStarName(), new Random(11).GenerateRandomStarName());
            Assert.Equal(new StarNameGenerator(11).GenerateMultipleStarNames(4), new Random(11).GenerateMultipleStarNames(4));
        }

        [Fact]
        public void ExtensionsRejectNullAndNegative()
        {
            Random? rand = null;

            Assert.Throws<ArgumentNullException>(() => rand!.GenerateRandomStarName());
            Assert.Throws<ArgumentNullException>(() => rand!.GenerateMultipleStarNames(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Random(1).GenerateMultipleStarNames(-1));
        }
    }
}
