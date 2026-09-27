using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Generates the names of real stars. Implemented by <see cref="StarNameGenerator"/>; register it with a
    /// dependency injection container or mock it in tests.
    /// </summary>
    public interface IStarNameGenerator
    {
        /// <summary>
        /// Generates the name of a named star: an IAU proper name or a Bayer or Flamsteed designation, for example
        /// <c>Betelgeuse</c>, <c>Tau Ceti</c> or <c>61 Cygni</c>.
        /// </summary>
        string GenerateRandomStarName();

        /// <summary>Generates an IAU-approved proper star name, for example <c>Sirius</c>.</summary>
        string GenerateRandomProperStarName();

        /// <summary>
        /// Generates the catalogue designation of a real star from the Henry Draper or Hipparcos catalogue,
        /// for example <c>HD 209458</c> or <c>HIP 70890</c>.
        /// </summary>
        string GenerateRandomCatalogStarName();

        /// <summary>Generates <paramref name="numberOfNames"/> names of named stars.</summary>
        /// <param name="numberOfNames">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="numberOfNames"/> is negative.</exception>
        IEnumerable<string> GenerateMultipleStarNames(int numberOfNames);
    }
}
