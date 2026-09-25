using System;
using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Lets any <see cref="Random"/> generate people's names directly. Each call wraps the instance in a
    /// <see cref="PersonNameGenerator"/>, so a seeded <see cref="Random"/> gives reproducible names.
    /// </summary>
    public static class RandomPersonNameExtensions
    {
        /// <summary>Generates a first name of either gender; see <see cref="IPersonNameGenerator.GenerateRandomFirstName"/>.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public static string GenerateRandomFirstName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomFirstName();
        }

        /// <summary>Generates a last name.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public static string GenerateRandomLastName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomLastName();
        }

        /// <summary>Generates a female first name.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public static string GenerateRandomFemaleFirstName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomFemaleFirstName();
        }

        /// <summary>Generates a male first name.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public static string GenerateRandomMaleFirstName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomMaleFirstName();
        }

        /// <summary>Generates a female first name followed by a space and a last name.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public static string GenerateRandomFemaleFirstAndLastName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomFemaleFirstAndLastName();
        }

        /// <summary>Generates a male first name followed by a space and a last name.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public static string GenerateRandomMaleFirstAndLastName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomMaleFirstAndLastName();
        }

        /// <summary>Generates a first name of either gender followed by a space and a last name.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public static string GenerateRandomFirstAndLastName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomFirstAndLastName();
        }

        /// <summary>Generates <paramref name="numberOfNames"/> first-and-last names of either gender.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <param name="numberOfNames">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfNames"/> is negative.</exception>
        public static IEnumerable<string> GenerateMultipleFirstAndLastNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleFirstAndLastNames(numberOfNames);
        }

        /// <summary>Generates <paramref name="numberOfNames"/> last names.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <param name="numberOfNames">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfNames"/> is negative.</exception>
        public static IEnumerable<string> GenerateMultipleLastNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleLastNames(numberOfNames);
        }

        /// <summary>Generates <paramref name="numberOfNames"/> female first-and-last names.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <param name="numberOfNames">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfNames"/> is negative.</exception>
        public static IEnumerable<string> GenerateMultipleFemaleFirstAndLastNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleFemaleFirstAndLastNames(numberOfNames);
        }

        /// <summary>Generates <paramref name="numberOfNames"/> male first-and-last names.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <param name="numberOfNames">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfNames"/> is negative.</exception>
        public static IEnumerable<string> GenerateMultipleMaleFirstAndLastNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleMaleFirstAndLastNames(numberOfNames);
        }

        /// <summary>Generates <paramref name="numberOfNames"/> female first names.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <param name="numberOfNames">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfNames"/> is negative.</exception>
        public static IEnumerable<string> GenerateMultipleFemaleFirstNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleFemaleFirstNames(numberOfNames);
        }

        /// <summary>Generates <paramref name="numberOfNames"/> male first names.</summary>
        /// <param name="rand">The random number generator to draw from.</param>
        /// <param name="numberOfNames">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfNames"/> is negative.</exception>
        public static IEnumerable<string> GenerateMultipleMaleFirstNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleMaleFirstNames(numberOfNames);
        }
    }
}
