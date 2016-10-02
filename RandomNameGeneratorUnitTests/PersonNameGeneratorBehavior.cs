using System.Linq;
using RandomNameGeneratorLibrary;
using Xunit;

namespace RandomNameGeneratorUnitTests
{    
    public class PersonNameGeneratorBehavior
    {
        private readonly PersonNameGenerator _personGenerator;

        public PersonNameGeneratorBehavior()
        {
            _personGenerator = new PersonNameGenerator();

        }

        [Fact]
        public void GetTheProperNumberOfRequestedNamesWithoutRepeats()
        {
            var result = _personGenerator.GenerateMultipleFirstAndLastNames(2).AsQueryable();

            Assert.Equal(result.Count(), 2);
            Assert.NotEqual(result.First(), result.Last());
        }

        [Fact]
        public void WhenAFirstAndLastNameAreGeneratedTogetherTheyShouldHaveASpaceBetweenThem()
        {
            var result = _personGenerator.GenerateRandomFirstAndLastName();

            Assert.NotNull(result);
            Assert.NotEqual(result.IndexOf(' '), -1);
        }

        [Fact]
        public void WhenMultipleFemaleFirstAndLastNamesAreGeneratedTogetherTheyShouldHaveASpaceBetweenThem()
        {
            var result = _personGenerator.GenerateMultipleFemaleFirstAndLastNames(2).First();

            Assert.NotNull(result);
            Assert.NotEqual(result.IndexOf(' '), -1);
        }
    }
}