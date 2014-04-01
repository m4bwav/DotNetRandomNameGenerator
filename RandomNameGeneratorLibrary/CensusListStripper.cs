using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RandomNameGeneratorLibrary
{
    public class CensusListStripper
    {
        public void StripStatisticsFromPersonNameFile(string nameFilePath, string nameStrippedFilePath)
        {
            StripStatisticsAndSaveFile(nameFilePath, nameStrippedFilePath, ExtractPersonNameStrings);
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
                var str2 = ConvertToStandardCasing(str1.Split(new[]
                {
                    ' '
                })[0]);
                stringBuilder.AppendLine(str2);
            }
            return stringBuilder;
        }

        private static string ConvertToStandardCasing(string uppercaseName)
        {
            var str = uppercaseName.ToLower();

            return str[0].ToString().ToUpper() + str.Remove(0, 1);
        }

        public static string RemoveDigits(string key)
        {
            return Regex.Replace(key, "\\d", "");
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
                throw new ArgumentOutOfRangeException("minusState");

            var townClassification = GetTownClassification(minusState, false);

            var startIndex = minusState.IndexOf(townClassification);

            return minusState.Remove(startIndex);
        }

        private static string GetTownClassification(string source, bool throwExceptionOnStrangePlaceName)
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
            if (source.Contains("Lexington-Fayette") || source.Contains("Anaconda-Deer Lodge County") ||
                (source.Contains("Carson City") || source.Contains("Lynchburg, Moore County")) ||
                (source.Contains("comunidad") || source.Contains("urbana") || !throwExceptionOnStrangePlaceName))
                return ".";
            throw new ArgumentOutOfRangeException("cannot find town classification in " + source);
        }

        public void StripStatisticsFromPlaceNameFile(string placeFilePath, string placeStrippedFilePath)
        {
            StripStatisticsAndSaveFile(placeFilePath, placeStrippedFilePath, ExtractPlaceNameStrings);
        }
    }
}