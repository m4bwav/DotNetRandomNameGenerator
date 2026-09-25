using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Generates random people's names. Implemented by <see cref="PersonNameGenerator"/>; register it with a
    /// dependency injection container or mock it in tests.
    /// </summary>
    public interface IPersonNameGenerator
    {
        /// <summary>Generates a first name of either gender followed by a space and a last name, for example <c>Mark Rogers</c>.</summary>
        string GenerateRandomFirstAndLastName();

        /// <summary>Generates <paramref name="count"/> names as <see cref="GenerateRandomFirstAndLastName"/> does.</summary>
        /// <param name="count">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        IEnumerable<string> GenerateMultipleFirstAndLastNames(int count);

        /// <summary>Generates <paramref name="count"/> last names.</summary>
        /// <param name="count">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        IEnumerable<string> GenerateMultipleLastNames(int count);

        /// <summary>Generates <paramref name="count"/> names as <see cref="GenerateRandomFemaleFirstAndLastName"/> does.</summary>
        /// <param name="count">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        IEnumerable<string> GenerateMultipleFemaleFirstAndLastNames(int count);

        /// <summary>Generates <paramref name="count"/> names as <see cref="GenerateRandomMaleFirstAndLastName"/> does.</summary>
        /// <param name="count">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        IEnumerable<string> GenerateMultipleMaleFirstAndLastNames(int count);

        /// <summary>Generates <paramref name="count"/> female first names.</summary>
        /// <param name="count">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        IEnumerable<string> GenerateMultipleFemaleFirstNames(int count);

        /// <summary>Generates <paramref name="count"/> male first names.</summary>
        /// <param name="count">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        IEnumerable<string> GenerateMultipleMaleFirstNames(int count);

        /// <summary>Generates a last name, for example <c>Rogers</c>.</summary>
        string GenerateRandomLastName();

        /// <summary>
        /// Picks male or female with equal probability, then generates a first name from that list. Because the male
        /// list is shorter, any one male name is about 3.5 times likelier than any one female name.
        /// </summary>
        string GenerateRandomFirstName();

        /// <summary>Generates a female first name, for example <c>Jane</c>.</summary>
        string GenerateRandomFemaleFirstName();

        /// <summary>Generates a male first name, for example <c>John</c>.</summary>
        string GenerateRandomMaleFirstName();

        /// <summary>Generates a female first name followed by a space and a last name.</summary>
        string GenerateRandomFemaleFirstAndLastName();

        /// <summary>Generates a male first name followed by a space and a last name.</summary>
        string GenerateRandomMaleFirstAndLastName();
    }
}
