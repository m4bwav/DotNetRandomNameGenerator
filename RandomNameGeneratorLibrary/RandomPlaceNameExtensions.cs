using System;
using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    public static class RandomPlaceNameExtensions
    {
        public static string GenerateRandomPlaceName(this Random rand)
        {
            return new PlaceNameGenerator(rand).GenerateRandomPlaceName();
        }

        public static IEnumerable<string> GenerateMultiplePlaceNames(this Random rand, int numberOfNames)
        {
            return new PlaceNameGenerator(rand).GenerateMultiplePlaceNames(numberOfNames);
        }
    }
}