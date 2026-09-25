using System;
using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Generates random people's names from the 1990 US Census name frequency files (frequencies stripped, so
    /// every entry on a list is equally likely). The lists are parsed once per process, on first use.
    /// </summary>
    /// <remarks>
    /// A generator created with the default constructor is safe to share between threads. A generator created
    /// around a caller-supplied <see cref="Random"/> is only as thread-safe as that instance, which for
    /// <see cref="Random"/> means not at all: use one generator per thread.
    /// </remarks>
    public class PersonNameGenerator : BaseNameGenerator, IPersonNameGenerator
    {
        private const string MaleFile = "dist.male.first.stripped";
        private const string FemaleFile = "dist.female.first.stripped";
        private const string LastNameFile = "dist.all.last.stripped";

        private static readonly Lazy<string[]> MaleFirstNameList = new Lazy<string[]>(() => ReadResourceByLine(MaleFile));
        private static readonly Lazy<string[]> FemaleFirstNameList = new Lazy<string[]>(() => ReadResourceByLine(FemaleFile));
        private static readonly Lazy<string[]> LastNameList = new Lazy<string[]>(() => ReadResourceByLine(LastNameFile));

        /// <summary>Creates a generator with a fresh, well-seeded random number generator.</summary>
        public PersonNameGenerator()
        {
        }

        /// <summary>Creates a generator that draws from <paramref name="randGen"/>.</summary>
        /// <param name="randGen">The random number generator to draw from. A seeded instance gives reproducible names.</param>
        /// <exception cref="ArgumentNullException"><paramref name="randGen"/> is null.</exception>
        public PersonNameGenerator(Random randGen) : base(randGen)
        {
        }

        /// <summary>Creates a generator that produces the same sequence of names for the same <paramref name="seed"/>.</summary>
        /// <param name="seed">The seed for the underlying <see cref="Random"/>.</param>
        public PersonNameGenerator(int seed) : base(new Random(seed))
        {
        }

        /// <summary>The 1,219 male first names the generator draws from, in Title case.</summary>
        public static IReadOnlyList<string> MaleFirstNames => MaleFirstNameList.Value;

        /// <summary>The 4,275 female first names the generator draws from, in Title case.</summary>
        public static IReadOnlyList<string> FemaleFirstNames => FemaleFirstNameList.Value;

        /// <summary>The 88,799 last names the generator draws from, in Title case.</summary>
        public static IReadOnlyList<string> LastNames => LastNameList.Value;

        private bool RandomlyPickIfNameIsMale => RandGen.Next(0, 2) == 0;

        /// <inheritdoc />
        public string GenerateRandomFirstAndLastName()
        {
            return GenerateRandomFirstName() + ' ' + GenerateRandomLastName();
        }

        /// <inheritdoc />
        public string GenerateRandomLastName()
        {
            return Pick(LastNameList.Value);
        }

        /// <inheritdoc />
        public string GenerateRandomFirstName()
        {
            return RandomlyPickIfNameIsMale
                ? GenerateRandomMaleFirstName()
                : GenerateRandomFemaleFirstName();
        }

        /// <inheritdoc />
        public string GenerateRandomFemaleFirstName()
        {
            return Pick(FemaleFirstNameList.Value);
        }

        /// <inheritdoc />
        public string GenerateRandomMaleFirstName()
        {
            return Pick(MaleFirstNameList.Value);
        }

        /// <inheritdoc />
        public string GenerateRandomFemaleFirstAndLastName()
        {
            return GenerateRandomFemaleFirstName() + ' ' + GenerateRandomLastName();
        }

        /// <inheritdoc />
        public string GenerateRandomMaleFirstAndLastName()
        {
            return GenerateRandomMaleFirstName() + ' ' + GenerateRandomLastName();
        }

        /// <inheritdoc />
        public IEnumerable<string> GenerateMultipleFirstAndLastNames(int count)
        {
            return Generate(count, GenerateRandomFirstAndLastName);
        }

        /// <inheritdoc />
        public IEnumerable<string> GenerateMultipleLastNames(int count)
        {
            return Generate(count, GenerateRandomLastName);
        }

        /// <inheritdoc />
        public IEnumerable<string> GenerateMultipleFemaleFirstAndLastNames(int count)
        {
            return Generate(count, GenerateRandomFemaleFirstAndLastName);
        }

        /// <inheritdoc />
        public IEnumerable<string> GenerateMultipleMaleFirstAndLastNames(int count)
        {
            return Generate(count, GenerateRandomMaleFirstAndLastName);
        }

        /// <inheritdoc />
        public IEnumerable<string> GenerateMultipleFemaleFirstNames(int count)
        {
            return Generate(count, GenerateRandomFemaleFirstName);
        }

        /// <inheritdoc />
        public IEnumerable<string> GenerateMultipleMaleFirstNames(int count)
        {
            return Generate(count, GenerateRandomMaleFirstName);
        }

        private string Pick(string[] names)
        {
            return names[RandGen.Next(0, names.Length)];
        }

        private static List<string> Generate(int count, Func<string> pick)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), count, "The number of names to generate cannot be negative.");

            var list = new List<string>(count);

            for (var index = 0; index < count; ++index)
                list.Add(pick());

            return list;
        }
    }
}
