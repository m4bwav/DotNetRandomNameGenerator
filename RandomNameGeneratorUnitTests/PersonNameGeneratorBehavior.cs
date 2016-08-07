using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomNameGeneratorLibrary;

namespace RandomNameGeneratorUnitTests
{
    [TestClass]
    public class PersonNameGeneratorBehavior
    {
        private PersonNameGenerator _personGenerator;

        [TestInitialize]
        public void Setup()
        {
            _personGenerator = new PersonNameGenerator();
        }

        [TestMethod]
        public void GetTheProperNumberOfRequestedNamesWithoutRepeats()
        {
            var result = _personGenerator.GenerateMultipleFirstAndLastNames(2).AsQueryable();

            Assert.AreEqual(result.Count(), 2);
            Assert.AreNotEqual(result.First(), result.Last());
        }

        [TestMethod]
        public void WhenAFirstAndLastNameAreGeneratedTogetherTheyShouldHaveASpaceBetweenThem()
        {
            var result = _personGenerator.GenerateRandomFirstAndLastName();

            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.IndexOf(' '), -1);
        }

        [TestMethod]
        public void WhenMultipleFemaleFirstAndLastNamesAreGeneratedTogetherTheyShouldHaveASpaceBetweenThem()
        {
            var result = _personGenerator.GenerateMultipleFemaleFirstAndLastNames(2).First();

            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.IndexOf(' '), -1);
        }
    }
}