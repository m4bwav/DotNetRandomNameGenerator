using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    public interface IPlaceNameGenerator
    {
        string GenerateRandomPlaceName();

        IEnumerable<string> GenerateMultiplePlaceNames(int numberOfNames);
    }
}