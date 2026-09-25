using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// The one-off tool that turned the raw US Census files into the embedded <c>Resources.*.stripped</c> lists.
    /// It is not part of the name-generating API and will be removed in 3.0; a maintained copy lives in the
    /// repository's <c>tools/CensusTools</c> console project.
    /// </summary>
    [Obsolete("CensusListStripper is a build-time tool, not part of the name-generating API, and will be removed in 3.0. The maintained copy is the tools/CensusTools console project in the repository (https://github.com/m4bwav/DotNetRandomNameGenerator).")]
    public class CensusListStripper
    {
        /// <summary>Strips the frequency columns from a 1990 Census name file, keeping one Title-case name per line.</summary>
        /// <param name="nameFilePath">Path of the Census file (<c>dist.male.first</c>, <c>dist.female.first</c> or <c>dist.all.last</c>).</param>
        /// <param name="nameStrippedFilePath">Path to write the stripped list to.</param>
        public void StripStatisticsFromPersonNameFile(string nameFilePath, string nameStrippedFilePath)
        {
            StripStatisticsAndSaveFile(nameFilePath, nameStrippedFilePath, ExtractPersonNameStrings);
        }

        /// <summary>Strips the state, population and classification columns from the Census 2000 places file, keeping one place name per line.</summary>
        /// <param name="placeFilePath">Path of the Census <c>places2k.txt</c> file.</param>
        /// <param name="placeStrippedFilePath">Path to write the stripped list to.</param>
        public void StripStatisticsFromPlaceNameFile(string placeFilePath, string placeStrippedFilePath)
        {
            StripStatisticsAndSaveFile(placeFilePath, placeStrippedFilePath, ExtractPlaceNameStrings);
        }

        /// <summary>Removes every decimal digit from <paramref name="key"/>.</summary>
        /// <param name="key">The text to strip digits from.</param>
        public static string RemoveDigits(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return Regex.Replace(key, "\\d", "");
        }

        private static void StripStatisticsAndSaveFile(string nameFilePath, string nameStrippedFilePath,
            Func<string[], StringBuilder> stripFunction)
        {
            var strArray = File.ReadAllLines(nameFilePath);
            var stringBuilder = stripFunction(strArray);
            File.WriteAllText(nameStrippedFilePath, stringBuilder.ToString());
        }

        private static StringBuilder ExtractPersonNameStrings(IEnumerable<string> names)
        {
            var stringBuilder = new StringBuilder();
            foreach (var str1 in names)
            {
                var str2 = ConvertToStandardCasing(str1.Split(' ')[0]);
                stringBuilder.AppendLine(str2);
            }
            return stringBuilder;
        }

        private static string ConvertToStandardCasing(string uppercaseName)
        {
            var str = uppercaseName.ToLowerInvariant();

            return str.Substring(0, 1).ToUpperInvariant() + str.Substring(1);
        }

        private static StringBuilder ExtractPlaceNameStrings(IEnumerable<string> names)
        {
            var stringBuilder = new StringBuilder();
            foreach (var key in names)
            {
                var str = RemoveTrailingTextOnPlaceName(RemoveDigits(key).Remove(0, 2)).Trim();
                stringBuilder.AppendLine(str);
            }
            return stringBuilder;
        }

        private static string RemoveTrailingTextOnPlaceName(string minusState)
        {
            if (string.IsNullOrWhiteSpace(minusState))
                throw new ArgumentOutOfRangeException(nameof(minusState));

            var townClassification = GetTownClassification(minusState);

            var startIndex = minusState.IndexOf(townClassification, StringComparison.Ordinal);

            return startIndex < 0 ? minusState : minusState.Remove(startIndex);
        }

        private static string GetTownClassification(string source)
        {
            if (source.Contains("town"))
                return "town";
            if (source.Contains("city"))
                return "city";
            if (source.Contains("CDP"))
                return "CDP";
            if (source.Contains("village"))
                return "village";
            if (source.Contains("municipality"))
                return "municipality";
            if (source.Contains("borough"))
                return "borough";
            if (source.Contains("(balance)"))
                return "(balance)";
            return ".";
        }
    }
}
