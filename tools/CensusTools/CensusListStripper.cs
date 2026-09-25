using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CensusTools
{
    /// <summary>
    /// Turns the raw US Census files into the one-name-per-line lists embedded in RandomNameGeneratorLibrary.
    /// Person files: 1990 name frequency files (dist.male.first, dist.female.first, dist.all.last), first column
    /// kept and Title-cased. Place file: Census 2000 places2k.txt, read as Latin-1 (the source encoding; reading
    /// it as UTF-8 is what produced the U+FFFD characters in the 2.0 place list), state code and numeric
    /// columns dropped, the classification word (town, city, CDP, ...) and everything after it removed, and
    /// duplicates across states collapsed so every place is equally likely.
    /// </summary>
    public static class CensusListStripper
    {
        private static readonly string[] Classifications = { " town", " city", " CDP", " village", " municipality", " borough", " (balance)" };

        public static void StripPersonNameFile(string nameFilePath, string strippedFilePath)
        {
            var names = File.ReadLines(nameFilePath, Encoding.Latin1)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => ToTitleCase(line.Split(' ')[0]));

            WriteList(strippedFilePath, Distinct(names));
        }

        public static void StripPlaceNameFile(string placeFilePath, string strippedFilePath)
        {
            var names = File.ReadLines(placeFilePath, Encoding.Latin1)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(ExtractPlaceName)
                .Where(name => name.Length != 0);

            WriteList(strippedFilePath, Distinct(names));
        }

        /// <summary>Extracts the place name from one places2k.txt row.</summary>
        /// <remarks>
        /// Row layout (fixed width): 2-letter state, 2-digit state FIPS, 5-digit place FIPS, 64-char name with
        /// classification, then population, housing units, land and water area, latitude and longitude.
        /// </remarks>
        public static string ExtractPlaceName(string row)
        {
            if (row == null) throw new ArgumentNullException(nameof(row));

            var body = row.Length > 9 ? row.Substring(9) : row;
            var digitsStart = IndexOfFirstDigit(body);
            var name = (digitsStart < 0 ? body : body.Substring(0, digitsStart)).Trim();

            foreach (var classification in Classifications)
            {
                var index = name.IndexOf(classification, StringComparison.Ordinal);
                if (index >= 0)
                    return name.Substring(0, index).Trim();
            }

            // Puerto Rico rows ("zona urbana", "comunidad") and a few consolidated cities have no classification.
            return name;
        }

        private static int IndexOfFirstDigit(string text)
        {
            for (var i = 0; i < text.Length; i++)
                if (char.IsDigit(text[i]))
                    return i;
            return -1;
        }

        private static string ToTitleCase(string name)
        {
            if (name.Length == 0) return name;
            var lower = name.ToLowerInvariant();
            return char.ToUpperInvariant(lower[0]) + lower.Substring(1);
        }

        private static IEnumerable<string> Distinct(IEnumerable<string> names)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var name in names)
                if (seen.Add(name))
                    yield return name;
        }

        private static void WriteList(string path, IEnumerable<string> names)
        {
            // LF line endings and UTF-8 without BOM, matching .gitattributes and what BaseNameGenerator expects.
            using var writer = new StreamWriter(path, false, new UTF8Encoding(false)) { NewLine = "\n" };
            foreach (var name in names)
                writer.WriteLine(name);
        }
    }
}
