using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    public interface IPersonNameGenerator
    {
        string GenerateRandomFirstAndLastName();

        IEnumerable<string> GenerateMultipleFirstAndLastNames(int count);

        IEnumerable<string> GenerateMultipleLastNames(int count);

        IEnumerable<string> GenerateMultipleFemaleFirstAndLastNames(int count);

        IEnumerable<string> GenerateMultipleMaleFirstAndLastNames(int count);

        IEnumerable<string> GenerateMultipleFemaleFirstNames(int count);

        IEnumerable<string> GenerateMultipleMaleFirstNames(int count);

        string GenerateRandomLastName();

        string GenerateRandomFirstName();

        string GenerateRandomFemaleFirstName();

        string GenerateRandomMaleFirstName();

        string GenerateRandomFemaleFirstAndLastName();

        string GenerateRandomMaleFirstAndLastName();
    }
}