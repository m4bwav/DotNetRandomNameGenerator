using System;
using System.Collections.Generic;
using System.Linq;
using RandomNameGeneratorLibrary;
using Xunit;

namespace RandomNameGeneratorUnitTests
{
    public class PlaceNameBehavior
    {
        [Fact]
        public void ShouldGenerateRandomName()
        {
            var name = new PlaceNameGenerator().GenerateRandomPlaceName();

            Assert.False(string.IsNullOrWhiteSpace(name));
        }

        [Fact]
        public void ShouldGenerateSameNameIfSameRandomGenerator()
        {
            var generator1 = new PlaceNameGenerator(new Random(42));
            var generator2 = new PlaceNameGenerator(new Random(42));

            Assert.Equal(generator1.GenerateRandomPlaceName(), generator2.GenerateRandomPlaceName());
        }

        [Fact]
        public void SeedConstructorMatchesSeededRandomConstructor()
        {
            var fromSeed = new PlaceNameGenerator(7).GenerateMultiplePlaceNames(5);
            var fromRandom = new PlaceNameGenerator(new Random(7)).GenerateMultiplePlaceNames(5);

            Assert.Equal(fromRandom, fromSeed);
        }

        // 2.3.0 regenerated the list from places2k.txt with tools/CensusTools. The 2014 stripper cut a name at the first
        // "town", "city", "village", ... anywhere in it, so Georgetown was listed as "George" and Felicity as "Feli".
        [Theory]
        [InlineData("Georgetown")]
        [InlineData("Allentown")]
        [InlineData("Hagerstown")]
        [InlineData("Middletown")]
        [InlineData("Newtown")]
        [InlineData("Felicity")]
        [InlineData("Quakertown")]
        public void PlaceNamesKeepWordsThatContainAClassification(string name)
        {
            Assert.Contains(name, PlaceNameGenerator.PlaceNames);
        }

        [Theory]
        [InlineData("Feli")]
        [InlineData("Hagers")]
        [InlineData("Gilber")]
        [InlineData("G")]
        [InlineData("H")]
        [InlineData("New Middle")]
        public void PlaceNamesHaveNoTruncatedEntries(string truncated)
        {
            Assert.DoesNotContain(truncated, PlaceNameGenerator.PlaceNames);
        }

        [Fact]
        public void PlaceNamesEndWithoutAClassificationWord()
        {
            var words = new[] { " town", " city", " CDP", " village", " municipality", " borough", " (balance)" };

            Assert.DoesNotContain(PlaceNameGenerator.PlaceNames, n => words.Any(w => n.EndsWith(w, StringComparison.Ordinal)));
        }

        [Fact]
        public void GenerateMultiplePlaceNamesReturnsTheRequestedCountFromTheList()
        {
            var places = new HashSet<string>(PlaceNameGenerator.PlaceNames);
            var names = new PlaceNameGenerator(9).GenerateMultiplePlaceNames(50).ToList();

            Assert.Equal(50, names.Count);
            Assert.All(names, name => Assert.Contains(name, places));
        }

        [Fact]
        public void ZeroCountReturnsEmpty()
        {
            Assert.Empty(new PlaceNameGenerator(1).GenerateMultiplePlaceNames(0));
        }

        [Fact]
        public void NegativeCountThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlaceNameGenerator(1).GenerateMultiplePlaceNames(-1));
        }

        [Fact]
        public void NullRandomThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new PlaceNameGenerator(null!));
        }

        [Fact]
        public void DefaultGeneratorsCreatedInATightLoopDoNotRepeatEachOther()
        {
            var names = new List<string>();
            for (var i = 0; i < 20; i++)
                names.Add(new PlaceNameGenerator().GenerateRandomPlaceName());

            Assert.True(names.Distinct().Count() > 1, "every default generator produced the same name");
        }
    }
}
