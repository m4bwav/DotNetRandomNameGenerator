using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Generates random US place names. Implemented by <see cref="PlaceNameGenerator"/>; register it with a
    /// dependency injection container or mock it in tests.
    /// </summary>
    public interface IPlaceNameGenerator
    {
        /// <summary>Generates a place name, for example <c>Hoboken</c>.</summary>
        string GenerateRandomPlaceName();

        /// <summary>Generates <paramref name="numberOfNames"/> place names.</summary>
        /// <param name="numberOfNames">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="numberOfNames"/> is negative.</exception>
        IEnumerable<string> GenerateMultiplePlaceNames(int numberOfNames);
    }
}
