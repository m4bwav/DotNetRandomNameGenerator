using System;
using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    public static class RandomPersonNameExtensions
    {
        public static string GenerateRandomFirstName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomFirstName();
        }

        public static string GenerateRandomLastName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomLastName();
        }

        public static string GenerateRandomFemaleFirstName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomFemaleFirstName();
        }

        public static string GenerateRandomMaleFirstName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomMaleFirstName();
        }

        public static string GenerateRandomFemaleFirstAndLastName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomFemaleFirstAndLastName();
        }


        public static string GenerateRandomMaleFirstAndLastName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomMaleFirstAndLastName();
        }


        public static string GenerateRandomFirstAndLastName(this Random rand)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));

            return new PersonNameGenerator(rand).GenerateRandomFirstAndLastName();
        }

        public static IEnumerable<string> GenerateMultipleFirstAndLastNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleFirstAndLastNames(numberOfNames);
        }

        public static IEnumerable<string> GenerateMultipleLastNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleLastNames(numberOfNames);
        }

        public static IEnumerable<string> GenerateMultipleFemaleFirstAndLastNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleFemaleFirstAndLastNames(numberOfNames);
        }

        public static IEnumerable<string> GenerateMultipleMaleFirstAndLastNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleMaleFirstAndLastNames(numberOfNames);
        }

        public static IEnumerable<string> GenerateMultipleFemaleFirstNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleFemaleFirstNames(numberOfNames);
        }

        public static IEnumerable<string> GenerateMultipleMaleFirstNames(this Random rand, int numberOfNames)
        {
            if (rand == null) throw new ArgumentNullException(nameof(rand));
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames));

            return new PersonNameGenerator(rand).GenerateMultipleMaleFirstNames(numberOfNames);
        }
    }
}