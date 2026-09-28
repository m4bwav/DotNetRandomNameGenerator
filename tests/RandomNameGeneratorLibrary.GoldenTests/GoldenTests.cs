using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
            var placesPath = Path.Combine(dir, "2.3.0.places." + Runtime + ".json");
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
    /// list, whose 2.3.0 answers are pinned in tests/Golden/2.3.0.places.&lt;runtime&gt;.json (plan D3).
    /// </summary>
    public class GoldenTests : IClassFixture<GoldenRun>
    {
        // Only a case whose name says it involves places may be a place exception (plan D3).
        private static readonly Regex PlaceCase = new Regex("Place|places2k|Shared Random|non-ASCII", RegexOptions.CultureInvariant);

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
                Assert.Matches(PlaceCase, p.Key);
                Assert.False(GoldenRun.JsonEqual(_run.Recorded[p.Key], p.Value), p.Key + " equals 2.2.0 and needs no exception");
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
