using System;
using System.Linq;
using RandomNameGeneratorLibrary;
using Xunit;

namespace RandomNameGeneratorUnitTests
{
    public class RandomExtensionBehavior
    {
        [Fact]
        public void CanGetARandomPlaceNameFromARandomObject()
        {
            var name = new Random().GenerateRandomPlaceName();

            Assert.NotNull(name);
        }

        [Fact]
        public void CanGetARandomPersonNameFromARandomObject()
        {
            var name = new Random().GenerateRandomFirstAndLastName();

            Assert.NotNull(name);
        }

        [Fact]
        public void ExtensionsAreDeterministicWithASeed()
        {
            Assert.Equal(new Random(42).GenerateRandomFirstAndLastName(), new Random(42).GenerateRandomFirstAndLastName());
            Assert.Equal(new Random(42).GenerateRandomFemaleFirstName(), new Random(42).GenerateRandomFemaleFirstName());
            Assert.Equal(new Random(42).GenerateRandomMaleFirstName(), new Random(42).GenerateRandomMaleFirstName());
            Assert.Equal(new Random(42).GenerateRandomLastName(), new Random(42).GenerateRandomLastName());
            Assert.Equal(new Random(42).GenerateRandomPlaceName(), new Random(42).GenerateRandomPlaceName());
            Assert.Equal(new Random(42).GenerateMultipleFirstAndLastNames(3), new Random(42).GenerateMultipleFirstAndLastNames(3));
            Assert.Equal(new Random(42).GenerateMultiplePlaceNames(3), new Random(42).GenerateMultiplePlaceNames(3));
        }

        [Fact]
        public void ExtensionsMatchTheGeneratorWithTheSameSeed()
        {
            Assert.Equal(new PersonNameGenerator(11).GenerateRandomFemaleFirstAndLastName(), new Random(11).GenerateRandomFemaleFirstAndLastName());
            Assert.Equal(new PlaceNameGenerator(11).GenerateMultiplePlaceNames(4), new Random(11).GenerateMultiplePlaceNames(4));
        }

        [Fact]
        public void MultipleExtensionsReturnTheRequestedCount()
        {
            var rand = new Random(5);

            Assert.Equal(4, rand.GenerateMultipleFirstAndLastNames(4).Count());
            Assert.Equal(4, rand.GenerateMultipleLastNames(4).Count());
            Assert.Equal(4, rand.GenerateMultipleFemaleFirstAndLastNames(4).Count());
            Assert.Equal(4, rand.GenerateMultipleMaleFirstAndLastNames(4).Count());
            Assert.Equal(4, rand.GenerateMultipleFemaleFirstNames(4).Count());
            Assert.Equal(4, rand.GenerateMultipleMaleFirstNames(4).Count());
            Assert.Equal(4, rand.GenerateMultiplePlaceNames(4).Count());
            Assert.Empty(rand.GenerateMultiplePlaceNames(0));
        }

        [Fact]
        public void NullRandomThrows()
        {
            Random rand = null!;

            Assert.Throws<ArgumentNullException>(() => rand.GenerateRandomFirstName());
            Assert.Throws<ArgumentNullException>(() => rand.GenerateRandomLastName());
            Assert.Throws<ArgumentNullException>(() => rand.GenerateRandomFemaleFirstName());
            Assert.Throws<ArgumentNullException>(() => rand.GenerateRandomMaleFirstName());
            Assert.Throws<ArgumentNullException>(() => rand.GenerateRandomFirstAndLastName());
            Assert.Throws<ArgumentNullException>(() => rand.GenerateRandomFemaleFirstAndLastName());
            Assert.Throws<ArgumentNullException>(() => rand.GenerateRandomMaleFirstAndLastName());
            Assert.Throws<ArgumentNullException>(() => rand.GenerateMultipleFirstAndLastNames(1));
            Assert.Throws<ArgumentNullException>(() => rand.GenerateMultipleLastNames(1));
            Assert.Throws<ArgumentNullException>(() => rand.GenerateMultipleFemaleFirstAndLastNames(1));
            Assert.Throws<ArgumentNullException>(() => rand.GenerateMultipleMaleFirstAndLastNames(1));
            Assert.Throws<ArgumentNullException>(() => rand.GenerateMultipleFemaleFirstNames(1));
            Assert.Throws<ArgumentNullException>(() => rand.GenerateMultipleMaleFirstNames(1));
            Assert.Throws<ArgumentNullException>(() => rand.GenerateRandomPlaceName());
            Assert.Throws<ArgumentNullException>(() => rand.GenerateMultiplePlaceNames(1));
        }

        [Fact]
        public void NegativeCountThrowsOnExtensions()
        {
            var rand = new Random(5);

            Assert.Throws<ArgumentOutOfRangeException>(() => rand.GenerateMultipleFirstAndLastNames(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => rand.GenerateMultiplePlaceNames(-1));
        }
    }
}
