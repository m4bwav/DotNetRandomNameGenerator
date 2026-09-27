using System;
using System.Collections.Generic;
using System.Globalization;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Generates the names of real stars: IAU proper names, Bayer and Flamsteed designations of every star in the
    /// Yale Bright Star Catalogue, and catalogue numbers from the Henry Draper and Hipparcos catalogues. Every
    /// entry of a list is equally likely. The lists are parsed once per process, on first use.
    /// </summary>
    /// <remarks>
    /// A generator created with the default constructor is safe to share between threads. A generator created
    /// around a caller-supplied <see cref="Random"/> is only as thread-safe as that instance, which for
    /// <see cref="Random"/> means not at all: use one generator per thread.
    /// </remarks>
    public class StarNameGenerator : BaseNameGenerator, IStarNameGenerator
    {
        private const string ProperNameFile = "stars.proper.stripped";
        private const string DesignationFile = "stars.designations.stripped";
        private const string HipparcosGapFile = "stars.hipgaps.stripped";

        // The Henry Draper catalogue numbers its stars HD 1 to HD 225300 without gaps.
        private const int HenryDraperCount = 225300;

        private static readonly Lazy<string[]> ProperNameList = new Lazy<string[]>(() => ReadResourceByLine(ProperNameFile));
        private static readonly Lazy<string[]> DesignationList = new Lazy<string[]>(() => ReadResourceByLine(DesignationFile));
        private static readonly Lazy<string[]> StarNameList = new Lazy<string[]>(CombineNamedStars);
        private static readonly Lazy<int[]> HipparcosNumbers = new Lazy<int[]>(ReadHipparcosNumbers);

        /// <summary>Creates a generator with a fresh, well-seeded random number generator.</summary>
        public StarNameGenerator()
        {
        }

        /// <summary>Creates a generator that draws from <paramref name="randGen"/>.</summary>
        /// <param name="randGen">The random number generator to draw from. A seeded instance gives reproducible names.</param>
        /// <exception cref="ArgumentNullException"><paramref name="randGen"/> is null.</exception>
        public StarNameGenerator(Random randGen) : base(randGen)
        {
        }

        /// <summary>Creates a generator that produces the same sequence of names for the same <paramref name="seed"/>.</summary>
        /// <param name="seed">The seed for the underlying <see cref="Random"/>.</param>
        public StarNameGenerator(int seed) : base(new Random(seed))
        {
        }

        /// <summary>
        /// The 640 proper star names approved by the IAU Working Group on Star Names, with their diacritics
        /// (for example <c>Sirius</c>, <c>Rosalíadecastro</c>).
        /// </summary>
        public static IReadOnlyList<string> ProperStarNames => ProperNameList.Value;

        /// <summary>
        /// The 3,076 Bayer and Flamsteed designations in the Yale Bright Star Catalogue, which holds every star
        /// visible to the naked eye, written out in full: <c>Alpha¹ Centauri</c>, <c>Tau Ceti</c>, <c>61 Cygni</c>.
        /// A star with a Bayer letter is listed by it rather than by its Flamsteed number.
        /// </summary>
        public static IReadOnlyList<string> DesignatedStarNames => DesignationList.Value;

        /// <summary>
        /// Every name <see cref="GenerateRandomStarName"/> draws from: <see cref="ProperStarNames"/> followed by
        /// <see cref="DesignatedStarNames"/>.
        /// </summary>
        public static IReadOnlyList<string> StarNames => StarNameList.Value;

        /// <summary>
        /// How many catalogue designations <see cref="GenerateRandomCatalogStarName"/> draws from: the 225,300
        /// Henry Draper stars plus the 118,218 Hipparcos stars.
        /// </summary>
        public static int CatalogStarNameCount => HenryDraperCount + HipparcosNumbers.Value.Length;

        /// <inheritdoc />
        public string GenerateRandomStarName()
        {
            var names = StarNameList.Value;

            return names[RandGen.Next(0, names.Length)];
        }

        /// <inheritdoc />
        public string GenerateRandomProperStarName()
        {
            var names = ProperNameList.Value;

            return names[RandGen.Next(0, names.Length)];
        }

        /// <inheritdoc />
        public string GenerateRandomCatalogStarName()
        {
            var hipparcos = HipparcosNumbers.Value;
            var index = RandGen.Next(0, HenryDraperCount + hipparcos.Length);

            return index < HenryDraperCount
                ? "HD " + (index + 1).ToString(CultureInfo.InvariantCulture)
                : "HIP " + hipparcos[index - HenryDraperCount].ToString(CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public IEnumerable<string> GenerateMultipleStarNames(int numberOfNames)
        {
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames), numberOfNames, "The number of names to generate cannot be negative.");

            var list = new List<string>(numberOfNames);

            for (var index = 0; index < numberOfNames; ++index)
                list.Add(GenerateRandomStarName());

            return list;
        }

        private static string[] CombineNamedStars()
        {
            var proper = ProperNameList.Value;
            var designated = DesignationList.Value;
            var all = new string[proper.Length + designated.Length];
            proper.CopyTo(all, 0);
            designated.CopyTo(all, proper.Length);

            return all;
        }

        // The resource stores the highest Hipparcos number on its first line, then every number below it that the
        // catalogue skips; 2,198 gaps are far smaller than 118,218 numbers.
        private static int[] ReadHipparcosNumbers()
        {
            var lines = ReadResourceByLine(HipparcosGapFile);
            var highest = int.Parse(lines[0], CultureInfo.InvariantCulture);
            var numbers = new int[highest - (lines.Length - 1)];
            var gap = 1;
            var count = 0;

            for (var number = 1; number <= highest; ++number)
            {
                if (gap < lines.Length && int.Parse(lines[gap], CultureInfo.InvariantCulture) == number)
                {
                    ++gap;
                    continue;
                }

                numbers[count++] = number;
            }

            return numbers;
        }
    }
}
