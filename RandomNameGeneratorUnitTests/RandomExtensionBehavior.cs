using System;
using RandomNameGeneratorLibrary;
using Xunit;

namespace RandomNameGeneratorUnitTests
{
    public class RandomExtensionBehavior
    {
        [Fact]
        public void CanGetARandomPlaceNameFromARandomObject()
        {
            var rand = new Random();

            var name = rand.GenerateRandomPlaceName();

            Assert.NotNull(name);
        }

        [Fact]
        public void CanGetARandomPersonNameFromARandomObject()
        {
            var rand = new Random();

            var name = rand.GenerateRandomFirstAndLastName();

            Assert.NotNull(name);
        }
    }
}