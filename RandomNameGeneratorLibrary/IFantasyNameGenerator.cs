using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Generates names for fantasy settings from word grammars: places, factions, war-bands, trade surnames,
    /// bynames and epithets. Implemented by <see cref="FantasyNameGenerator"/>; register it with a dependency
    /// injection container or mock it in tests.
    /// </summary>
    public interface IFantasyNameGenerator
    {
        /// <summary>
        /// Generates a place name: an adjective and a structure, sometimes followed by "of the" and a two-word
        /// feature, for example <c>Broken Tower of the Ash Hollow</c> or <c>Sunken Abbey</c>.
        /// </summary>
        string GenerateRandomFantasyPlaceName();

        /// <summary>
        /// Generates a place name that fits <paramref name="landform"/>, for example <c>Windy Tor</c> for
        /// <see cref="Landform.Hill"/> or <c>Saltmere Strand</c> for <see cref="Landform.Coast"/>.
        /// </summary>
        /// <param name="landform">The kind of land the place stands on.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="landform"/> is not a defined value.</exception>
        string GenerateRandomLandformPlaceName(Landform landform);

        /// <summary>
        /// Generates a faction or sub-tribe name: a totem animal and a group noun, for example
        /// <c>the Grey Heron Clan</c>.
        /// </summary>
        string GenerateRandomFactionName();

        /// <summary>
        /// Generates a war-band name compounded from a verb and a body part, for example <c>Splitskull</c> or
        /// <c>Breakfang</c>.
        /// </summary>
        string GenerateRandomWarBandName();

        /// <summary>
        /// Generates a surname from an occupation, sometimes in an older spelling, for example <c>Smith</c>,
        /// <c>Smyth</c>, <c>Taylor</c> or <c>Tailor</c>.
        /// </summary>
        string GenerateRandomTradeSurname();

        /// <summary>
        /// Generates a census first name followed by a byname, for example <c>Anna the Bold</c>.
        /// </summary>
        string GenerateRandomBynameName();

        /// <summary>
        /// Generates a keeper's epithet for <paramref name="name"/>: the name, "the" and a trait or deed, for
        /// example <c>Bram the Honest</c> or <c>Bram the Lantern-Bearer</c>.
        /// </summary>
        /// <param name="name">The name to give an epithet.</param>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is null or white space.</exception>
        string GenerateRandomKeeperEpithet(string name);

        /// <summary>Generates <paramref name="numberOfNames"/> place names with <see cref="GenerateRandomFantasyPlaceName"/>.</summary>
        /// <param name="numberOfNames">How many names to generate; zero gives an empty sequence.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="numberOfNames"/> is negative.</exception>
        IEnumerable<string> GenerateMultipleFantasyPlaceNames(int numberOfNames);
    }
}
