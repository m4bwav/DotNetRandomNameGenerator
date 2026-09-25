using System;

namespace CensusTools
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 3 && args[0] == "person")
            {
                CensusListStripper.StripPersonNameFile(args[1], args[2]);
                return 0;
            }

            if (args.Length == 3 && args[0] == "place")
            {
                CensusListStripper.StripPlaceNameFile(args[1], args[2]);
                return 0;
            }

            Console.Error.WriteLine("Usage:");
            Console.Error.WriteLine("  CensusTools person <dist.male.first|dist.female.first|dist.all.last> <output.stripped>");
            Console.Error.WriteLine("  CensusTools place  <places2k.txt> <output.stripped>");
            Console.Error.WriteLine("Sources: https://www.census.gov/topics/population/genealogy/data/1990_census/1990_census_namefiles.html");
            Console.Error.WriteLine("         https://www.census.gov/geographies/reference-files/2000/demo/2000-places.html (places2k.txt)");
            return 1;
        }
    }
}
