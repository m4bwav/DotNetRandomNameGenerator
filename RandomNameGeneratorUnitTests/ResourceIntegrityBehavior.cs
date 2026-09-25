using System.Collections.Generic;
using System.Linq;
using RandomNameGeneratorLibrary;
using Xunit;

namespace RandomNameGeneratorUnitTests
{
    /// <summary>
    /// Guards the embedded census lists. These tests would have caught the two data problems fixed in 2.1.0:
    /// 49 Puerto Rico place names carrying U+FFFD from a bad Latin-1 decode, and 8,502 duplicate place rows.
    /// </summary>
    public class ResourceIntegrityBehavior
    {
        public static IEnumerable<object[]> Lists()
        {
            yield return new object[] { "male first names", PersonNameGenerator.MaleFirstNames, 1219 };
            yield return new object[] { "female first names", PersonNameGenerator.FemaleFirstNames, 4275 };
            yield return new object[] { "last names", PersonNameGenerator.LastNames, 88799 };
            yield return new object[] { "place names", PlaceNameGenerator.PlaceNames, 16873 };
        }

        [Theory]
        [MemberData(nameof(Lists))]
        public void ListHasTheExpectedCount(string list, IReadOnlyList<string> names, int expected)
        {
            Assert.True(expected == names.Count, list + ": expected " + expected + " entries, found " + names.Count);
        }

        [Theory]
        [MemberData(nameof(Lists))]
        public void ListHasNoBlankOrUntrimmedEntries(string list, IReadOnlyList<string> names, int expected)
        {
            Assert.All(names, name => Assert.False(string.IsNullOrWhiteSpace(name), list + " has a blank entry"));
            Assert.All(names, name => Assert.True(name == name.Trim(), list + " has an untrimmed entry: '" + name + "'"));
            _ = expected;
        }

        [Theory]
        [MemberData(nameof(Lists))]
        public void ListHasNoReplacementCharacter(string list, IReadOnlyList<string> names, int expected)
        {
            var broken = names.Where(name => name.Contains('�')).ToList();

            Assert.True(broken.Count == 0, list + " has " + broken.Count + " entries with U+FFFD, e.g. '" + broken.FirstOrDefault() + "'");
            _ = expected;
        }

        [Theory]
        [MemberData(nameof(Lists))]
        public void ListHasNoDuplicates(string list, IReadOnlyList<string> names, int expected)
        {
            var duplicates = names.GroupBy(name => name).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

            Assert.True(duplicates.Count == 0, list + " has " + duplicates.Count + " duplicated entries, e.g. '" + duplicates.FirstOrDefault() + "'");
            _ = expected;
        }

        [Fact]
        public void PersonListsAreAsciiTitleCase()
        {
            var lists = new[] { PersonNameGenerator.MaleFirstNames, PersonNameGenerator.FemaleFirstNames, PersonNameGenerator.LastNames };

            foreach (var names in lists)
            {
                Assert.All(names, name => Assert.True(name.All(c => c < 128), "non-ASCII person name: " + name));
                Assert.All(names, name => Assert.True(char.IsUpper(name[0]) && name.Skip(1).All(char.IsLower), "not Title case: " + name));
            }
        }

        [Fact]
        public void PuertoRicoPlaceNamesKeepTheirAccents()
        {
            Assert.Contains("Bayamón zona urbana", PlaceNameGenerator.PlaceNames);
            Assert.Contains("Mayagüez zona urbana", PlaceNameGenerator.PlaceNames);
            Assert.Contains("Río Cañas Abajo comunidad", PlaceNameGenerator.PlaceNames);
        }
    }
}
