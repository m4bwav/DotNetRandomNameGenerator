using System;
using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Lets any <see cref="Random"/> generate star names directly. Each call wraps the instance in a
    /// <see cref="StarNameGenerator"/>, so a seeded <see cref="Random"/> gives reproducible names.
    /// </summary>
    public static class RandomStarNameExtensions
    {
        /// <summary>Generates the name of a named star, for example <c>Betelgeuse</c> or <c>Tau Ceti</c>.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public static string GenerateRandomStarName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new StarNameGenerator(rand).GenerateRandomStarName();
        }

        /// <summary>Generates <paramref name="numberOfNames"/> names of named stars.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <param name="numberOfNames">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfNames"/> is negative.</exception>
        public static IEnumerable<string> GenerateMultipleStarNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new StarNameGenerator(rand).GenerateMultipleStarNames(numberOfNames);
        }
    }
}
