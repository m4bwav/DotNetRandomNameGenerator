using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Shared plumbing for <see cref="PersonNameGenerator"/> and <see cref="PlaceNameGenerator"/>:
    /// holds the <see cref="Random"/> the generator draws from and reads the embedded census lists.
    /// This class is an implementation detail and is not meant to be used or derived from by callers;
    /// it stays public only for binary compatibility with 1.x and 2.0.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class BaseNameGenerator
    {
        private const string ResourcePathPrefix = "RandomNameGeneratorLibrary.Resources.";

#if !NET6_0_OR_GREATER
        // On .NET Framework, new Random() is seeded from the tick count, so two generators created in the same
        // tick produce the same sequence (GitHub issue #7). One process-wide source hands out distinct seeds.
        private static readonly Random SeedSource = new Random();
        private static readonly object SeedLock = new object();
#endif

        /// <summary>
        /// The random number generator every name is drawn from. When it was supplied by the caller it is not
        /// thread-safe: use one generator per thread.
        /// </summary>
        protected readonly Random RandGen;

        /// <summary>Creates a generator with a fresh, well-seeded random number generator.</summary>
        protected BaseNameGenerator() : this(CreateDefaultRandom())
        {
        }

        /// <summary>Creates a generator that draws from <paramref name="randGen"/>.</summary>
        /// <param name="randGen">The random number generator to draw from. A seeded instance gives reproducible names.</param>
        /// <exception cref="ArgumentNullException"><paramref name="randGen"/> is null.</exception>
        protected BaseNameGenerator(Random randGen)
        {
            RandGen = randGen ?? throw new ArgumentNullException(nameof(randGen));
        }

        private static Random CreateDefaultRandom()
        {
#if NET6_0_OR_GREATER
            return Random.Shared;
#else
            lock (SeedLock)
            {
                return new Random(SeedSource.Next());
            }
#endif
        }

        /// <summary>
        /// Reads an embedded census list, one entry per line, skipping blank lines.
        /// </summary>
        /// <param name="resourceFileName">The file name after the <c>RandomNameGeneratorLibrary.Resources.</c> prefix.</param>
        /// <returns>The non-blank lines of the resource, in file order.</returns>
        /// <exception cref="InvalidOperationException">The resource is not embedded in the assembly.</exception>
        protected static string[] ReadResourceByLine(string resourceFileName)
        {
            var resourceName = ResourcePathPrefix + resourceFileName;
            using (var stream = typeof(BaseNameGenerator).Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException("Embedded resource '" + resourceName + "' was not found in " + typeof(BaseNameGenerator).Assembly.FullName + ".");

                var list = new List<string>();
                using (var streamReader = new StreamReader(stream))
                {
                    string? line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line.Length != 0)
                            list.Add(line);
                    }
                }

                return list.ToArray();
            }
        }
    }
}
