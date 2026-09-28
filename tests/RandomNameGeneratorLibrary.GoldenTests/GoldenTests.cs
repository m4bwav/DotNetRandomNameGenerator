using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using GoldenCapture;
using Xunit;

namespace RandomNameGeneratorLibrary.GoldenTests
{
    /// <summary>
    /// Runs the capture's 426 cases once and loads the recordings of this runtime: the published 2.2.0's answers and the
    /// ruled exception file for the regenerated place list (plan D3), when present.
    /// </summary>
    public sealed class GoldenRun
    {
#if NET
        public const string Runtime = "net10.0";
#else
        public const string Runtime = "net48";
#endif

        public GoldenRun()
        {
            var saved = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            try
            {
                Actual = Cases.Run().ToList();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = saved;
            }

            var dir = Path.Combine(AppContext.BaseDirectory, "Golden");
            Recorded = Load(Path.Combine(dir, "2.2.0." + Runtime + "-windows.json"));
            var placesPath = Path.Combine(AppContext.BaseDirectory, "Exceptions", "2.3.0.places." + Runtime + ".json");
            Places = File.Exists(placesPath) ? Load(placesPath) : new Dictionary<string, JsonElement>(StringComparer.Ordinal);

            // Writes the cases that now differ from 2.2.0, for a ruled exception file. Never set in CI; the file it
            // writes is reviewed by hand and must pass Place_exceptions_are_place_cases below.
            var write = Environment.GetEnvironmentVariable("GOLDEN_WRITE_DIFFERENCES");
            if (!string.IsNullOrEmpty(write))
            {
                var root = new JsonObject();
                root.Add("note", "Answers of the 2.3.0 build that differ from 2.2.0 because the place list was regenerated (plan D3). Written by GOLDEN_WRITE_DIFFERENCES from the release build, reviewed, never edited by hand.");
                root.Add("runtime", Runtime);
                var list = new List<object>();
                foreach (var c in Actual)
                {
                    var key = Key(c.Group, c.Name);
                    using var actual = JsonDocument.Parse(Json.Write(c.Result));
                    if (!JsonEqual(Recorded[key], actual.RootElement))
                    {
                        var o = new JsonObject();
                        o.Add("group", c.Group);
                        o.Add("name", c.Name);
                        o.Add("result", c.Result);
                        list.Add(o);
                    }
                }

                root.Add("cases", list);
                File.WriteAllText(write, Json.Write(root), new UTF8Encoding(false));
            }
        }

        public List<Case> Actual { get; }

        public Dictionary<string, JsonElement> Recorded { get; }

        public Dictionary<string, JsonElement> Places { get; }

        public static string Key(string group, string name)
        {
            return group + " | " + name;
        }

        private static Dictionary<string, JsonElement> Load(string path)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            return doc.RootElement.GetProperty("cases").EnumerateArray()
                .ToDictionary(c => Key(c.GetProperty("group").GetString()!, c.GetProperty("name").GetString()!), c => c.GetProperty("result").Clone(), StringComparer.Ordinal);
        }

        public static bool JsonEqual(JsonElement a, JsonElement b)
        {
            if (a.ValueKind != b.ValueKind)
            {
                return false;
            }

            switch (a.ValueKind)
            {
                case JsonValueKind.Object:
                    var ap = a.EnumerateObject().ToList();
                    var bp = b.EnumerateObject().ToList();
                    return ap.Count == bp.Count && ap.Zip(bp, (x, y) => x.Name == y.Name && JsonEqual(x.Value, y.Value)).All(ok => ok);
                case JsonValueKind.Array:
                    var ai = a.EnumerateArray().ToList();
                    var bi = b.EnumerateArray().ToList();
                    return ai.Count == bi.Count && ai.Zip(bi, JsonEqual).All(ok => ok);
                case JsonValueKind.String:
                    return string.Equals(a.GetString(), b.GetString(), StringComparison.Ordinal);
                default:
                    return a.GetRawText() == b.GetRawText();
            }
        }
    }

    /// <summary>
    /// The golden contract (plan D1): every answer the published 2.2.0 gave on this runtime, from
    /// tests/Golden/2.2.0.&lt;runtime&gt;-windows.json, apart from the one ruled exception: cases that depend on the place
    /// list, whose 2.3.0 answers are pinned in tests/RandomNameGeneratorLibrary.GoldenTests/Exceptions/2.3.0.places.&lt;runtime&gt;.json (plan D3).
    /// </summary>
    public class GoldenTests : IClassFixture<GoldenRun>
    {
        // The 35 cases the place-list rebuild may change (plan D3), exactly; any other difference fails.
        private static readonly HashSet<string> AllowedPlaceExceptions = new HashSet<string>(StringComparer.Ordinal)
        {
            "Resources | RandomNameGeneratorLibrary.Resources.places2k.txt.stripped",
            "Lists | PlaceNameGenerator.PlaceNames",
            "Lists | names with non-ASCII characters, all lists",
            "Seeded PlaceNameGenerator | GenerateRandomPlaceName seed 0",
            "Seeded PlaceNameGenerator | GenerateRandomPlaceName seed 1",
            "Seeded PlaceNameGenerator | GenerateRandomPlaceName seed 42",
            "Seeded PlaceNameGenerator | GenerateRandomPlaceName seed 12345",
            "Seeded PlaceNameGenerator | GenerateRandomPlaceName seed 20260924",
            "Seeded PlaceNameGenerator | GenerateRandomPlaceName seed -1",
            "Seeded PlaceNameGenerator | GenerateRandomPlaceName seed -12345",
            "Seeded PlaceNameGenerator | GenerateRandomPlaceName seed 2147483647",
            "Seeded PlaceNameGenerator | GenerateRandomPlaceName seed -2147483648",
            "Seeded large | GenerateRandomPlaceName x10000 seed 7",
            "Seeded large | GenerateRandomPlaceName x10000 seed 2026",
            "Multiple | PlaceNameGenerator.GenerateMultiplePlaceNames count 1",
            "Multiple | PlaceNameGenerator.GenerateMultiplePlaceNames count 3",
            "Multiple | PlaceNameGenerator.GenerateMultiplePlaceNames count 25",
            "Multiple | PlaceNameGenerator.GenerateMultiplePlaceNames count 10000",
            "Multiple | an interface call gives the same names, seed 99",
            "Random extensions | GenerateRandomPlaceName x10 on one Random, seed 5",
            "Random extensions | GenerateMultiplePlaceNames count 3 twice on one Random, seed 5",
            "Shared Random | person, place, star, extensions and the caller's own draws on one Random, seed 314",
            "Random calls | PlaceNameGenerator.GenerateRandomPlaceName, seed 2024",
            "Random calls | PlaceNameGenerator.GenerateMultiplePlaceNames count 3, seed 2024",
            "Random calls | Random.GenerateRandomPlaceName, seed 2024",
            "Random calls | Random.GenerateMultiplePlaceNames count 3, seed 2024",
            "Scripted Random | GenerateRandomPlaceName first",
            "Scripted Random | Random.GenerateRandomPlaceName first",
            "Scripted Random | GenerateRandomPlaceName last",
            "Scripted Random | Random.GenerateRandomPlaceName last",
            "Scripted Random | GenerateRandomPlaceName max (out of range)",
            "Scripted Random | Random.GenerateRandomPlaceName max (out of range)",
            "Scripted Random | GenerateRandomPlaceName min - 1 (out of range)",
            "Scripted Random | Random.GenerateRandomPlaceName min - 1 (out of range)",
            "Subclass | ReadResourceByLine places2k.txt.stripped",
        };

        // The resource SHA-256 of the tools/CensusTools output recorded in tools/CensusTools/SOURCES.md's rebuild.
        private const string PlaceListSha256 = "f409cb412ffce410eece666c38d9f4ba76bea119a720613ea16997dcdbb7d85e";

        private readonly GoldenRun _run;

        public GoldenTests(GoldenRun run)
        {
            _run = run;
        }

        [Fact]
        public void Every_recorded_case_runs()
        {
            var actual = _run.Actual.Select(c => GoldenRun.Key(c.Group, c.Name)).OrderBy(k => k, StringComparer.Ordinal).ToList();
            Assert.Equal(426, actual.Count);
            Assert.Equal(_run.Recorded.Keys.OrderBy(k => k, StringComparer.Ordinal), actual);
        }

        [Fact]
        public void Place_exceptions_are_place_cases_that_really_changed()
        {
            foreach (var p in _run.Places)
            {
                Assert.True(_run.Recorded.ContainsKey(p.Key), p.Key + " is not a recorded case");
                Assert.Contains(p.Key, AllowedPlaceExceptions);
                Assert.False(GoldenRun.JsonEqual(_run.Recorded[p.Key], p.Value), p.Key + " equals 2.2.0 and needs no exception");
            }
        }

        [Fact]
        public void The_place_exceptions_are_exactly_the_ruled_cases()
        {
            Assert.Equal(AllowedPlaceExceptions.OrderBy(k => k, StringComparer.Ordinal), _run.Places.Keys.OrderBy(k => k, StringComparer.Ordinal));
        }

        [Fact]
        public void Every_place_exception_matches_an_oracle_independent_of_the_generators()
        {
            var names = PlaceNameGenerator.PlaceNames;
            var n = names.Count;
            Func<Random, int, List<string>> draw = (r, count) => Enumerable.Range(0, count).Select(_ => names[r.Next(0, n)]).ToList();
            var checkedKeys = new HashSet<string>(StringComparer.Ordinal);

            JsonElement Ex(string key)
            {
                checkedKeys.Add(key);
                return _run.Places[key];
            }

            List<string> Strings(JsonElement e) => e.EnumerateArray().Select(x => x.GetString()!).ToList();

            // The data: the resource is the CensusTools rebuild, and every view of it agrees.
            var resource = Ex("Resources | RandomNameGeneratorLibrary.Resources.places2k.txt.stripped");
            Assert.Equal(PlaceListSha256, resource.GetProperty("sha256").GetString());
            Assert.Equal(n, resource.GetProperty("lineFeeds").GetInt32());
            var list = Ex("Lists | PlaceNameGenerator.PlaceNames");
            Assert.Equal(n, list.GetProperty("count").GetInt32());
            Assert.Equal(Sha(names), list.GetProperty("sha256").GetString());
            Assert.Equal(n, Ex("Subclass | ReadResourceByLine places2k.txt.stripped").GetProperty("count").GetInt32());
            var nonAscii = PersonNameGenerator.MaleFirstNames.Concat(PersonNameGenerator.FemaleFirstNames).Concat(PersonNameGenerator.LastNames)
                .Concat(names).Concat(StarNameGenerator.StarNames).Where(x => x.Any(c => c > (char)127)).Take(40).ToList();
            Assert.Equal(nonAscii, Strings(Ex("Lists | names with non-ASCII characters, all lists")));

            // Seeded draws: one Next(0, count) per name, as 2.2.0 recorded in the "Random calls" cases.
            foreach (var seed in new[] { 0, 1, 42, 12345, 20260924, -1, -12345, int.MaxValue, int.MinValue })
            {
                Assert.Equal(draw(new Random(seed), 10), Strings(Ex("Seeded PlaceNameGenerator | GenerateRandomPlaceName seed " + seed.ToString(CultureInfo.InvariantCulture))));
            }

            foreach (var seed in new[] { 7, 2026 })
            {
                var large = Ex("Seeded large | GenerateRandomPlaceName x10000 seed " + seed.ToString(CultureInfo.InvariantCulture));
                var expected = draw(new Random(seed), 10000);
                Assert.Equal(Sha(expected), large.GetProperty("sha256").GetString());
                Assert.Equal(expected.Distinct(StringComparer.Ordinal).Count(), large.GetProperty("distinct").GetInt32());
            }

            foreach (var count in new[] { 1, 3, 25 })
            {
                var multiple = Ex("Multiple | PlaceNameGenerator.GenerateMultiplePlaceNames count " + count.ToString(CultureInfo.InvariantCulture));
                Assert.Equal(draw(new Random(99), count), Strings(multiple.GetProperty("names")));
            }

            Assert.Equal(Sha(draw(new Random(99), 10000)), Ex("Multiple | PlaceNameGenerator.GenerateMultiplePlaceNames count 10000").GetProperty("sha256").GetString());
            Assert.Equal(draw(new Random(5), 10), Strings(Ex("Random extensions | GenerateRandomPlaceName x10 on one Random, seed 5")));
            var twice = Ex("Random extensions | GenerateMultiplePlaceNames count 3 twice on one Random, seed 5");
            var five = new Random(5);
            Assert.Equal(draw(five, 3), Strings(twice.GetProperty("first")));
            Assert.Equal(draw(five, 3), Strings(twice.GetProperty("second")));

            // Mixed cases: the place answers follow the oracle and every other answer equals 2.2.0's.
            var mixed = Strings(Ex("Multiple | an interface call gives the same names, seed 99"));
            var mixedOld = Strings(_run.Recorded["Multiple | an interface call gives the same names, seed 99"]);
            var places99 = draw(new Random(99), 4);
            Assert.Equal(places99.Take(3), mixed.Skip(3).Take(3));
            Assert.Equal(places99[3], mixed[10]);
            foreach (var i in Enumerable.Range(0, mixed.Count).Except(new[] { 3, 4, 5, 10 }))
            {
                Assert.Equal(mixedOld[i], mixed[i]);
            }

            var shared = _run.Places["Shared Random | person, place, star, extensions and the caller's own draws on one Random, seed 314"].EnumerateArray().ToList();
            checkedKeys.Add("Shared Random | person, place, star, extensions and the caller's own draws on one Random, seed 314");
            var sharedOld = _run.Recorded["Shared Random | person, place, star, extensions and the caller's own draws on one Random, seed 314"].EnumerateArray().ToList();
            Assert.Equal(sharedOld.Count, shared.Count);
            for (var i = 0; i < shared.Count; i++)
            {
                if (i % 6 != 1)
                {
                    Assert.Equal(sharedOld[i].GetRawText(), shared[i].GetRawText());
                }
            }

            // Recorded Random calls: the same draws against the new count, the same names the oracle gives.
            foreach (var key in new[] { "Random calls | PlaceNameGenerator.GenerateRandomPlaceName, seed 2024", "Random calls | Random.GenerateRandomPlaceName, seed 2024" })
            {
                var r = new Random(2024);
                var v = r.Next(0, n);
                var e = Ex(key);
                Assert.Equal(names[v], e.GetProperty("result").GetString());
                Assert.Equal(new[] { "Next(0," + n.ToString(CultureInfo.InvariantCulture) + ")=" + v.ToString(CultureInfo.InvariantCulture) }, Strings(e.GetProperty("calls")));
            }

            foreach (var key in new[] { "Random calls | PlaceNameGenerator.GenerateMultiplePlaceNames count 3, seed 2024", "Random calls | Random.GenerateMultiplePlaceNames count 3, seed 2024" })
            {
                var r = new Random(2024);
                var v = Enumerable.Range(0, 3).Select(_ => r.Next(0, n)).ToList();
                var e = Ex(key);
                Assert.Equal(v.Select(i => names[i]), Strings(e.GetProperty("result")));
                Assert.Equal(v.Select(i => "Next(0," + n.ToString(CultureInfo.InvariantCulture) + ")=" + i.ToString(CultureInfo.InvariantCulture)), Strings(e.GetProperty("calls")));
            }

            // Scripted bounds: 2.2.0's answers with the new count and last index (the first and last names did not change).
            foreach (var key in AllowedPlaceExceptions.Where(k => k.StartsWith("Scripted Random | ", StringComparison.Ordinal)))
            {
                var expected = _run.Recorded[key].GetRawText().Replace("16873", n.ToString(CultureInfo.InvariantCulture)).Replace("16872", (n - 1).ToString(CultureInfo.InvariantCulture));
                using var doc = JsonDocument.Parse(expected);
                Assert.True(GoldenRun.JsonEqual(doc.RootElement, Ex(key)), key);
            }

            Assert.Equal(AllowedPlaceExceptions.OrderBy(k => k, StringComparer.Ordinal), checkedKeys.OrderBy(k => k, StringComparer.Ordinal));
        }

        private static string Sha(IEnumerable<string> lines)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", lines))).Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
        }

        [Fact]
        public void Seeded_place_exceptions_are_the_new_list_indexed_by_the_same_draws()
        {
            // An oracle independent of PlaceNameGenerator's code: the 2.2.0 algorithm (one Next(0, count) per name, recorded
            // in the "Random calls" cases) applied to the regenerated list gives exactly the pinned answers.
            var seeded = _run.Places.Where(p => p.Key.StartsWith("Seeded PlaceNameGenerator | GenerateRandomPlaceName seed ", StringComparison.Ordinal)).ToList();
            Assert.Equal(9, seeded.Count);
            var names = PlaceNameGenerator.PlaceNames;
            Assert.Equal(16969, names.Count);
            foreach (var p in seeded)
            {
                var seed = int.Parse(p.Key.Substring(p.Key.LastIndexOf(' ') + 1), CultureInfo.InvariantCulture);
                var random = new Random(seed);
                var expected = Enumerable.Range(0, 10).Select(_ => names[random.Next(0, names.Count)]).ToList();
                Assert.Equal(expected, p.Value.EnumerateArray().Select(e => e.GetString()!).ToList());
            }
        }

        [Fact]
        public void Every_answer_matches_2_2_0_apart_from_the_place_exceptions()
        {
            var failures = new List<string>();
            foreach (var c in _run.Actual)
            {
                var key = GoldenRun.Key(c.Group, c.Name);
                var actualText = Json.Write(c.Result);
                using var actual = JsonDocument.Parse(actualText);
                var isPlace = _run.Places.TryGetValue(key, out var place);
                var expected = isPlace ? place : _run.Recorded[key];
                if (!GoldenRun.JsonEqual(expected, actual.RootElement))
                {
                    failures.Add(key + (isPlace ? " (place exception)" : string.Empty) + ": expected " + expected.GetRawText() + ", got " + actualText.Trim());
                }
            }

            Assert.True(failures.Count == 0, failures.Count + " case(s) differ:" + Environment.NewLine + string.Join(Environment.NewLine, failures.Take(40)));
        }
    }
}
