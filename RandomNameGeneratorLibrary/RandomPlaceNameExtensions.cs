using System;
using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Lets any <see cref="Random"/> generate place names directly. Each call wraps the instance in a
    /// <see cref="PlaceNameGenerator"/>, so a seeded <see cref="Random"/> gives reproducible names.
    /// </summary>
    public static class RandomPlaceNameExtensions
    {
        /// <summary>Generates a place name, for example <c>Hoboken</c>.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public static string GenerateRandomPlaceName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PlaceNameGenerator(rand).GenerateRandomPlaceName();
        }

        /// <summary>Generates <paramref name="numberOfNames"/> place names.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <param name="numberOfNames">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfNames"/> is negative.</exception>
        public static IEnumerable<string> GenerateMultiplePlaceNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PlaceNameGenerator(rand).GenerateMultiplePlaceNames(numberOfNames);
        }
    }
}
