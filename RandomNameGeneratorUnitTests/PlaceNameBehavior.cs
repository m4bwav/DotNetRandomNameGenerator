using System;
using RandomNameGeneratorLibrary;
using Xunit;

namespace RandomNameGeneratorUnitTests
{    
    public class PlaceNameBehavior
    {
        private readonly PlaceNameGenerator _placeGenerator;

        public PlaceNameBehavior()
        {
            _placeGenerator = new PlaceNameGenerator();

        }

        [Fact]
        public void ShouldGenerateRandomName()
        {
            var name = _placeGenerator.GenerateRandomPlaceName();

            Assert.False(string.IsNullOrWhiteSpace(name));
        }

        [Fact]
        public void ShouldGenerateSameNameIfSameRandomGenerator()
        {
            var personNameGenerator1 = new PersonNameGenerator(new Random(42));
            var personNameGenerator2 = new PersonNameGenerator(new Random(42));

            var firstName = personNameGenerator1.GenerateRandomFirstAndLastName();
            var secondName = personNameGenerator2.GenerateRandomFirstAndLastName();

            Assert.Equal(firstName, secondName);
        }
    }
}