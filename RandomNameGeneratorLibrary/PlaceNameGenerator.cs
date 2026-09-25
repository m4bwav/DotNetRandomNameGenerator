using System;
using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Generates random US place names from the Census 2000 places file (one entry per distinct name, so every
    /// place is equally likely). The list is parsed once per process, on first use.
    /// </summary>
    /// <remarks>
    /// A generator created with the default constructor is safe to share between threads. A generator created
    /// around a caller-supplied <see cref="Random"/> is only as thread-safe as that instance, which for
    /// <see cref="Random"/> means not at all: use one generator per thread.
    /// </remarks>
    public class PlaceNameGenerator : BaseNameGenerator, IPlaceNameGenerator
    {
        private const string PlaceNameFile = "places2k.txt.stripped";

        private static readonly Lazy<string[]> PlaceNameList = new Lazy<string[]>(() => ReadResourceByLine(PlaceNameFile));

        /// <summary>Creates a generator with a fresh, well-seeded random number generator.</summary>
        public PlaceNameGenerator()
        {
        }

        /// <summary>Creates a generator that draws from <paramref name="randGen"/>.</summary>
        /// <param name="randGen">The random number generator to draw from. A seeded instance gives reproducible names.</param>
        /// <exception cref="ArgumentNullException"><paramref name="randGen"/> is null.</exception>
        public PlaceNameGenerator(Random randGen) : base(randGen)
        {
        }

        /// <summary>Creates a generator that produces the same sequence of names for the same <paramref name="seed"/>.</summary>
        /// <param name="seed">The seed for the underlying <see cref="Random"/>.</param>
        public PlaceNameGenerator(int seed) : base(new Random(seed))
        {
        }

        /// <summary>
        /// The 16,873 distinct place names the generator draws from. Puerto Rico entries keep their Census
        /// suffix, for example <c>Bayamón zona urbana</c>.
        /// </summary>
        public static IReadOnlyList<string> PlaceNames => PlaceNameList.Value;

        /// <inheritdoc />
        public string GenerateRandomPlaceName()
        {
            var names = PlaceNameList.Value;

            return names[RandGen.Next(0, names.Length)];
        }

        /// <inheritdoc />
        public IEnumerable<string> GenerateMultiplePlaceNames(int numberOfNames)
        {
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames), numberOfNames, "The number of names to generate cannot be negative.");

            var list = new List<string>(numberOfNames);

            for (var index = 0; index < numberOfNames; ++index)
                list.Add(GenerateRandomPlaceName());

            return list;
        }
    }
}
