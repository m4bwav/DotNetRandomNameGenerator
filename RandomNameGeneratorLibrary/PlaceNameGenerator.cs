using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    public class PlaceNameGenerator : BaseNameGenerator, IPlaceNameGenerator
    {
        private const string PlaceNameFile = "places2k.txt.stripped";
        private static string[] _placeNames;

        public PlaceNameGenerator()
        {
            if (_placeNames != null)
                return;

            _placeNames = ReadResourceByLine(PlaceNameFile);
        }

        public string GenerateRandomPlaceName()
        {
            var index = RandGen.Next(0, _placeNames.Length);

            return _placeNames[index];
        }

        public IEnumerable<string> GenerateMultiplePlaceNames(int numberOfNames)
        {
            var list = new List<string>();

            for (var index = 0; index < numberOfNames; ++index)
                list.Add(GenerateRandomPlaceName());

            return list;
        }
    }
}