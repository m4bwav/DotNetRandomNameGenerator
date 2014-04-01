using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomNameGeneratorLibrary;

namespace RandomNameGeneratorUnitTests
{
    [TestClass]
    public class PlaceNameBehavior
    {
        private PlaceNameGenerator _placeGenerator;

        [TestInitialize]
        public void Setup()
        {
            _placeGenerator = new PlaceNameGenerator();    
        }

        [TestMethod]
        public void ShouldGenerateRandomName()
        {
            var name = _placeGenerator.GenerateRandomPlaceName();

            Assert.IsFalse(string.IsNullOrWhiteSpace(name));
        }
    }
}
