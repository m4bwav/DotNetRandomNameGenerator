using System;
using System.Linq;
using System.Text.RegularExpressions;
using RandomNameGeneratorLibrary;
using Xunit;

namespace RandomNameGeneratorUnitTests
{
    public class FantasyNameBehavior
    {
        private const int Draws = 200;

        private static void AssertSeeded(Func<FantasyNameGenerator, string> draw)
        {
            var a = new FantasyNameGenerator(42);
            var b = new FantasyNameGenerator(new Random(42));
            var first = Enumerable.Range(0, 20).Select(_ => draw(a)).ToList();
            var second = Enumerable.Range(0, 20).Select(_ => draw(b)).ToList();

            Assert.Equal(first, second);
            Assert.True(first.Distinct().Count() > 1, "a seeded generator should not repeat one name");
        }

        private static void AssertAllMatch(Func<FantasyNameGenerator, string> draw, string pattern)
        {
            var generator = new FantasyNameGenerator(7);
            for (var i = 0; i < Draws; i++)
            {
                var name = draw(generator);
                Assert.False(string.IsNullOrWhiteSpace(name));
                Assert.Matches(pattern, name);
            }
        }

        [Fact]
        public void DefaultConstructorGeneratesNames()
        {
            var generator = new FantasyNameGenerator();

            Assert.False(string.IsNullOrWhiteSpace(generator.GenerateRandomFantasyPlaceName()));
            Assert.False(string.IsNullOrWhiteSpace(generator.GenerateRandomBynameName()));
        }

        [Fact]
        public void NullRandomIsRejected()
        {
            Assert.Throws<ArgumentNullException>(() => new FantasyNameGenerator(null!));
        }

        [Fact]
        public void FantasyPlaceNameIsSeededAndShaped()
        {
            AssertSeeded(g => g.GenerateRandomFantasyPlaceName());
            AssertAllMatch(g => g.GenerateRandomFantasyPlaceName(), @"^[A-Z][a-z]+ [A-Z][a-z]+( of the [A-Z][a-z]+ [A-Z][a-z]+)?$");
        }

        [Fact]
        public void FantasyPlaceNamesSometimesHaveAFeatureAndSometimesNot()
        {
            var names = new FantasyNameGenerator(1).GenerateMultipleFantasyPlaceNames(Draws).ToList();

            Assert.Contains(names, n => n.Contains(" of the "));
            Assert.Contains(names, n => !n.Contains(" of the "));
        }

        [Fact]
        public void GenerateMultipleFantasyPlaceNamesReturnsTheCountAndMatchesSingleDraws()
        {
            var many = new FantasyNameGenerator(5).GenerateMultipleFantasyPlaceNames(10).ToList();
            var single = new FantasyNameGenerator(5);

            Assert.Equal(10, many.Count);
            Assert.Equal(many, Enumerable.Range(0, 10).Select(_ => single.GenerateRandomFantasyPlaceName()));
            Assert.Empty(new FantasyNameGenerator(5).GenerateMultipleFantasyPlaceNames(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new FantasyNameGenerator(5).GenerateMultipleFantasyPlaceNames(-1));
        }

        [Theory]
        [InlineData(Landform.Hill, "Hill|Tor|Knoll|Down|Rise|Ridge|Barrow|Brow|Mound|Heights")]
        [InlineData(Landform.River, "Ford|Bend|Water|Brook|Run|Rapids|Crossing|Falls|Weir|Banks")]
        [InlineData(Landform.Forest, "Wood|Grove|Weald|Thicket|Holt|Glade|Copse|Shaw|Greenwood|Tangle")]
        [InlineData(Landform.Marsh, "Fen|Mire|Bog|Marsh|Slough|Carr|Moss|Wallow|Sump|Flats")]
        [InlineData(Landform.Coast, "Strand|Cove|Bay|Point|Head|Haven|Sound|Shore|Cliffs|Harbor")]
        [InlineData(Landform.Mountain, "Peak|Crag|Pass|Spire|Horn|Summit|Scarp|Fang|Tooth|Col")]
        [InlineData(Landform.Plain, "Field|Lea|Moor|Heath|Steppe|Meadow|Plain|Prairie|Flat|Acre")]
        public void LandformPlaceNameEndsInAFittingWord(Landform landform, string words)
        {
            AssertSeeded(g => g.GenerateRandomLandformPlaceName(landform));
            AssertAllMatch(g => g.GenerateRandomLandformPlaceName(landform), "^[A-Z][a-z]+ (" + words + ")$");
        }

        [Fact]
        public void UnknownLandformIsRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FantasyNameGenerator(1).GenerateRandomLandformPlaceName((Landform)99));
        }

        [Fact]
        public void FactionNameIsSeededAndShaped()
        {
            AssertSeeded(g => g.GenerateRandomFactionName());
            AssertAllMatch(g => g.GenerateRandomFactionName(), "^the [A-Z][a-z]+ [A-Z][a-z]+ [A-Z][a-z]+$");
        }

        [Fact]
        public void WarBandNameIsOneCapitalizedCompound()
        {
            AssertSeeded(g => g.GenerateRandomWarBandName());
            AssertAllMatch(g => g.GenerateRandomWarBandName(), "^[A-Z][a-z]+[a-z]+$");
        }

        [Fact]
        public void TradeSurnameIsSeededAndIsAnOccupationOrItsVariant()
        {
            AssertSeeded(g => g.GenerateRandomTradeSurname());
            var allowed = FantasyNameGenerator.TradeSurnames.Concat(FantasyNameGenerator.TradeSurnames.Select(FantasyNameGenerator.ApplySpellingVariant)).ToList();
            var generator = new FantasyNameGenerator(3);
            var names = Enumerable.Range(0, Draws).Select(_ => generator.GenerateRandomTradeSurname()).ToList();

            Assert.All(names, n => Assert.Contains(n, allowed));
            Assert.Contains(names, n => !FantasyNameGenerator.TradeSurnames.Contains(n));
        }

        [Theory]
        [InlineData("Smith", "Smyth")]
        [InlineData("Taylor", "Tailor")]
        [InlineData("Clark", "Clarke")]
        [InlineData("Cook", "Cooke")]
        [InlineData("Miller", "Miler")]
        [InlineData("Turner", "Turnar")]
        [InlineData("Mason", "Mason")]
        public void ApplySpellingVariantFollowsTheRules(string surname, string expected)
        {
            Assert.Equal(expected, FantasyNameGenerator.ApplySpellingVariant(surname));
        }

        [Fact]
        public void ApplySpellingVariantRejectsNull()
        {
            Assert.Throws<ArgumentNullException>(() => FantasyNameGenerator.ApplySpellingVariant(null!));
        }

        [Fact]
        public void BynameNameIsACensusFirstNameAndAByname()
        {
            AssertSeeded(g => g.GenerateRandomBynameName());
            var generator = new FantasyNameGenerator(11);
            for (var i = 0; i < Draws; i++)
            {
                var name = generator.GenerateRandomBynameName();
                var match = Regex.Match(name, "^(.+) the [A-Z][a-z]+$");
                Assert.True(match.Success, name);
                var given = match.Groups[1].Value;
                Assert.True(PersonNameGenerator.MaleFirstNames.Contains(given) || PersonNameGenerator.FemaleFirstNames.Contains(given), given);
            }
        }

        [Fact]
        public void KeeperEpithetIsSeededAndShaped()
        {
            AssertSeeded(g => g.GenerateRandomKeeperEpithet("Bram"));
            AssertAllMatch(g => g.GenerateRandomKeeperEpithet("  Bram "), "^Bram the [A-Z][a-z]+(-[A-Z][a-z]+)?$");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void KeeperEpithetRejectsAMissingName(string? name)
        {
            Assert.Throws<ArgumentException>(() => new FantasyNameGenerator(1).GenerateRandomKeeperEpithet(name!));
        }

        [Fact]
        public void TheInterfaceExposesTheGenerator()
        {
            IFantasyNameGenerator generator = new FantasyNameGenerator(2);

            Assert.False(string.IsNullOrWhiteSpace(generator.GenerateRandomWarBandName()));
        }
    }
}
