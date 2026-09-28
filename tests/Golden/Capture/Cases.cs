#nullable disable
#pragma warning disable CS0618 // CensusListStripper and FileCompressor are obsolete since 2.1.0; the capture records them anyway.

// Every case of the golden capture of RandomNameGeneratorLibrary 2.2.0. Touches only public (and, through a subclass,
// protected) names, so the golden test compiles this file unchanged against the new library and compares its answers.
// Seeded generators are deterministic, so most cases record exact names; default (unseeded) generators are recorded
// only as properties that hold on every run. Text that depends on the machine is replaced inside each case, so the
// recording is portable: the work folder by {WORK}, the library's assembly identity by {ASSEMBLY}, and
// Environment.NewLine in files the library writes by {NL}.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using GoldenCapture.Fixtures;
using RandomNameGeneratorLibrary;

namespace GoldenCapture
{
    public sealed class Case
    {
        public string Group;
        public string Name;
        public object Result;
    }

    public static class Cases
    {
        private const int Draws = 10;
        private const char ReplacementChar = (char)0xFFFD;
        private static readonly int[] Seeds = { 0, 1, 42, 12345, 20260924, -1, -12345, int.MaxValue, int.MinValue };
        private static readonly int[] Counts = { 0, 1, 3, 25 };
        private static readonly int[] BadCounts = { -1, int.MinValue, int.MaxValue };
        private static readonly List<Case> All = new List<Case>();
        private static string _work;

        public static List<Case> Run()
        {
            All.Clear();
            _work = Path.Combine(Path.GetTempPath(), "rng-golden-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_work);
            try
            {
                ResourceCases();
                ListCases();
                SeededCases();
                ConstructorCases();
                MultipleCases();
                ExtensionCases();
                SharedRandomCases();
                RandomCallCases();
                ScriptedCases();
                DefaultCases();
                SubclassCases();
                CultureCases();
                CensusListStripperCases();
                FileCompressorCases();
            }
            finally
            {
                try
                {
                    Directory.Delete(_work, true);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            return All;
        }

        // ---------- helpers ----------

        private static void Add(string group, string name, Func<object> call)
        {
            object result;
            try
            {
                result = call();
            }
            catch (Exception e)
            {
                result = Describe(e);
            }

            All.Add(new Case { Group = group, Name = name, Result = result });
        }

        public static JsonObject Describe(Exception e)
        {
            var o = new JsonObject();
            o.Add("$throws", e.GetType().FullName + ": " + Normalize(FirstLine(e.Message)));
            var inner = new List<object>();
            for (var x = e.InnerException; x != null; x = x.InnerException)
            {
                inner.Add(x.GetType().FullName + ": " + Normalize(FirstLine(x.Message)));
            }

            if (inner.Count > 0)
            {
                o.Add("$inner", inner);
            }

            return o;
        }

        private static string Normalize(string text)
        {
            if (text == null)
            {
                return null;
            }

            text = text.Replace(typeof(PersonNameGenerator).Assembly.FullName, "{ASSEMBLY}");
            if (_work != null)
            {
                if (text.Contains(_work))
                {
                    // Paths under the work folder use / on every OS, so one recording serves Windows, Linux and macOS.
                    text = text.Replace(_work, "{WORK}").Replace(Path.DirectorySeparatorChar, '/');
                }
            }

            return text;
        }

        private static string FirstLine(string message)
        {
            if (message == null)
            {
                return "";
            }

            var i = message.IndexOfAny(new[] { (char)13, (char)10 });
            return i < 0 ? message : message.Substring(0, i);
        }

        // Runs one step inside a case and records its value or its exception, so later steps still run.
        private static object Try(Func<object> step)
        {
            try
            {
                return step();
            }
            catch (Exception e)
            {
                return Describe(e);
            }
        }

        private static string Sha256(byte[] bytes)
        {
            using (var h = SHA256.Create())
            {
                return string.Concat(h.ComputeHash(bytes).Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
            }
        }

        private static string Sha256(IEnumerable<string> lines)
        {
            return Sha256(Encoding.UTF8.GetBytes(string.Join("\n", lines)));
        }

        private static List<object> Strings(IEnumerable<string> values)
        {
            return values.Cast<object>().ToList();
        }

        private static List<object> Draw(int n, Func<string> pick)
        {
            var list = new List<object>();
            for (var i = 0; i < n; i++)
            {
                list.Add(pick());
            }

            return list;
        }

        private static string TypeName(Type t)
        {
            if (t.IsGenericType)
            {
                return t.Namespace + "." + t.Name + "[" + string.Join(",", t.GetGenericArguments().Select(TypeName)) + "]";
            }

            return t.FullName;
        }

        private static JsonObject Summary(IEnumerable<string> values)
        {
            var list = values.ToList();
            var o = new JsonObject();
            o.Add("type", TypeName(values.GetType()));
            o.Add("count", list.Count);
            o.Add("sha256", Sha256(list));
            o.Add("distinct", list.Distinct(StringComparer.Ordinal).Count());
            o.Add("first", Strings(list.Take(3)));
            return o;
        }

        private static string Path2(string name)
        {
            return Path.Combine(_work, name);
        }

        private static string ReadOutput(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
            var text = Encoding.UTF8.GetString(bytes, bom ? 3 : 0, bytes.Length - (bom ? 3 : 0));
            return (bom ? "{BOM}" : "") + text.Replace(Environment.NewLine, "{NL}");
        }

        private struct Method<T>
        {
            public string Name;
            public Func<T, string> Call;
        }

        private static Method<T> M<T>(string name, Func<T, string> call)
        {
            return new Method<T> { Name = name, Call = call };
        }

        private static readonly Method<PersonNameGenerator>[] PersonMethods =
        {
            M<PersonNameGenerator>("GenerateRandomFirstAndLastName", g => g.GenerateRandomFirstAndLastName()),
            M<PersonNameGenerator>("GenerateRandomFirstName", g => g.GenerateRandomFirstName()),
            M<PersonNameGenerator>("GenerateRandomLastName", g => g.GenerateRandomLastName()),
            M<PersonNameGenerator>("GenerateRandomFemaleFirstName", g => g.GenerateRandomFemaleFirstName()),
            M<PersonNameGenerator>("GenerateRandomMaleFirstName", g => g.GenerateRandomMaleFirstName()),
            M<PersonNameGenerator>("GenerateRandomFemaleFirstAndLastName", g => g.GenerateRandomFemaleFirstAndLastName()),
            M<PersonNameGenerator>("GenerateRandomMaleFirstAndLastName", g => g.GenerateRandomMaleFirstAndLastName()),
        };

        private static readonly Method<PlaceNameGenerator>[] PlaceMethods =
        {
            M<PlaceNameGenerator>("GenerateRandomPlaceName", g => g.GenerateRandomPlaceName()),
        };

        private static readonly Method<StarNameGenerator>[] StarMethods =
        {
            M<StarNameGenerator>("GenerateRandomStarName", g => g.GenerateRandomStarName()),
            M<StarNameGenerator>("GenerateRandomProperStarName", g => g.GenerateRandomProperStarName()),
            M<StarNameGenerator>("GenerateRandomCatalogStarName", g => g.GenerateRandomCatalogStarName()),
        };

        private struct Multiple<T>
        {
            public string Name;
            public Func<T, int, IEnumerable<string>> Call;
        }

        private static Multiple<T> MM<T>(string name, Func<T, int, IEnumerable<string>> call)
        {
            return new Multiple<T> { Name = name, Call = call };
        }

        private static readonly Multiple<PersonNameGenerator>[] PersonMultiples =
        {
            MM<PersonNameGenerator>("GenerateMultipleFirstAndLastNames", (g, n) => g.GenerateMultipleFirstAndLastNames(n)),
            MM<PersonNameGenerator>("GenerateMultipleLastNames", (g, n) => g.GenerateMultipleLastNames(n)),
            MM<PersonNameGenerator>("GenerateMultipleFemaleFirstAndLastNames", (g, n) => g.GenerateMultipleFemaleFirstAndLastNames(n)),
            MM<PersonNameGenerator>("GenerateMultipleMaleFirstAndLastNames", (g, n) => g.GenerateMultipleMaleFirstAndLastNames(n)),
            MM<PersonNameGenerator>("GenerateMultipleFemaleFirstNames", (g, n) => g.GenerateMultipleFemaleFirstNames(n)),
            MM<PersonNameGenerator>("GenerateMultipleMaleFirstNames", (g, n) => g.GenerateMultipleMaleFirstNames(n)),
        };

        private static readonly Method<Random>[] RandomExtensions =
        {
            M<Random>("GenerateRandomFirstName", r => r.GenerateRandomFirstName()),
            M<Random>("GenerateRandomLastName", r => r.GenerateRandomLastName()),
            M<Random>("GenerateRandomFemaleFirstName", r => r.GenerateRandomFemaleFirstName()),
            M<Random>("GenerateRandomMaleFirstName", r => r.GenerateRandomMaleFirstName()),
            M<Random>("GenerateRandomFemaleFirstAndLastName", r => r.GenerateRandomFemaleFirstAndLastName()),
            M<Random>("GenerateRandomMaleFirstAndLastName", r => r.GenerateRandomMaleFirstAndLastName()),
            M<Random>("GenerateRandomFirstAndLastName", r => r.GenerateRandomFirstAndLastName()),
            M<Random>("GenerateRandomPlaceName", r => r.GenerateRandomPlaceName()),
            M<Random>("GenerateRandomStarName", r => r.GenerateRandomStarName()),
        };

        private static readonly Multiple<Random>[] RandomMultipleExtensions =
        {
            MM<Random>("GenerateMultipleFirstAndLastNames", (r, n) => r.GenerateMultipleFirstAndLastNames(n)),
            MM<Random>("GenerateMultipleLastNames", (r, n) => r.GenerateMultipleLastNames(n)),
            MM<Random>("GenerateMultipleFemaleFirstAndLastNames", (r, n) => r.GenerateMultipleFemaleFirstAndLastNames(n)),
            MM<Random>("GenerateMultipleMaleFirstAndLastNames", (r, n) => r.GenerateMultipleMaleFirstAndLastNames(n)),
            MM<Random>("GenerateMultipleFemaleFirstNames", (r, n) => r.GenerateMultipleFemaleFirstNames(n)),
            MM<Random>("GenerateMultipleMaleFirstNames", (r, n) => r.GenerateMultipleMaleFirstNames(n)),
            MM<Random>("GenerateMultiplePlaceNames", (r, n) => r.GenerateMultiplePlaceNames(n)),
            MM<Random>("GenerateMultipleStarNames", (r, n) => r.GenerateMultipleStarNames(n)),
        };

        // ---------- the embedded data ----------

        private static void ResourceCases()
        {
            var asm = typeof(PersonNameGenerator).Assembly;
            var names = asm.GetManifestResourceNames().OrderBy(n => n, StringComparer.Ordinal).ToList();
            Add("Resources", "manifest resource names", () => Strings(names));
            foreach (var name in names)
            {
                Add("Resources", name, () =>
                {
                    using (var stream = asm.GetManifestResourceStream(name))
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        var b = ms.ToArray();
                        var o = new JsonObject();
                        o.Add("bytes", b.Length);
                        o.Add("sha256", Sha256(b));
                        o.Add("lineFeeds", b.Count(x => x == 10));
                        o.Add("carriageReturns", b.Count(x => x == 13));
                        o.Add("bom", b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF);
                        return o;
                    }
                });
            }
        }

        private static void ListCases()
        {
            ListCase("PersonNameGenerator.MaleFirstNames", () => PersonNameGenerator.MaleFirstNames);
            ListCase("PersonNameGenerator.FemaleFirstNames", () => PersonNameGenerator.FemaleFirstNames);
            ListCase("PersonNameGenerator.LastNames", () => PersonNameGenerator.LastNames);
            ListCase("PlaceNameGenerator.PlaceNames", () => PlaceNameGenerator.PlaceNames);
            ListCase("StarNameGenerator.ProperStarNames", () => StarNameGenerator.ProperStarNames);
            ListCase("StarNameGenerator.DesignatedStarNames", () => StarNameGenerator.DesignatedStarNames);
            ListCase("StarNameGenerator.StarNames", () => StarNameGenerator.StarNames);
            Add("Lists", "StarNames is ProperStarNames then DesignatedStarNames", () =>
                StarNameGenerator.StarNames.SequenceEqual(StarNameGenerator.ProperStarNames.Concat(StarNameGenerator.DesignatedStarNames), StringComparer.Ordinal));
            Add("Lists", "StarNameGenerator.CatalogStarNameCount", () => StarNameGenerator.CatalogStarNameCount);
            Add("Lists", "names with non-ASCII characters, all lists", () => Strings(
                PersonNameGenerator.MaleFirstNames.Concat(PersonNameGenerator.FemaleFirstNames).Concat(PersonNameGenerator.LastNames)
                    .Concat(PlaceNameGenerator.PlaceNames).Concat(StarNameGenerator.StarNames)
                    .Where(x => x.Any(c => c > (char)127)).Take(40)));
        }

        private static void ListCase(string name, Func<IReadOnlyList<string>> get)
        {
            Add("Lists", name, () =>
            {
                var a = get();
                var b = get();
                var o = new JsonObject();
                o.Add("type", TypeName(a.GetType()));
                o.Add("sameInstance", ReferenceEquals(a, b));
                o.Add("isReadOnly", Try(() => ((ICollection<string>)a).IsReadOnly));
                o.Add("count", a.Count);
                o.Add("sha256", Sha256(a));
                o.Add("distinct", a.Distinct(StringComparer.Ordinal).Count());
                o.Add("first", Strings(a.Take(3)));
                o.Add("last", Strings(a.Skip(Math.Max(0, a.Count - 3))));
                o.Add("nonAscii", a.Count(x => x.Any(c => c > (char)127)));
                o.Add("replacementChar", a.Count(x => x.IndexOf(ReplacementChar) >= 0));
                o.Add("blankOrPadded", a.Count(x => x.Length == 0 || x.Trim() != x));
                o.Add("maxLength", a.Max(x => x.Length));
                o.Add("indexer -1", Try(() => a[-1]));
                o.Add("indexer Count", Try(() => a[a.Count]));
                return o;
            });
        }

        // ---------- seeded generators ----------

        private static void SeededCases()
        {
            foreach (var seed in Seeds)
            {
                foreach (var m in PersonMethods)
                {
                    Add("Seeded PersonNameGenerator", m.Name + " seed " + seed.ToString(CultureInfo.InvariantCulture), () =>
                    {
                        var g = new PersonNameGenerator(seed);
                        return Draw(Draws, () => m.Call(g));
                    });
                }

                foreach (var m in PlaceMethods)
                {
                    Add("Seeded PlaceNameGenerator", m.Name + " seed " + seed.ToString(CultureInfo.InvariantCulture), () =>
                    {
                        var g = new PlaceNameGenerator(seed);
                        return Draw(Draws, () => m.Call(g));
                    });
                }

                foreach (var m in StarMethods)
                {
                    Add("Seeded StarNameGenerator", m.Name + " seed " + seed.ToString(CultureInfo.InvariantCulture), () =>
                    {
                        var g = new StarNameGenerator(seed);
                        return Draw(Draws, () => m.Call(g));
                    });
                }
            }

            Add("Seeded PersonNameGenerator", "every method in turn on one generator, seed 2024", () =>
            {
                var g = new PersonNameGenerator(2024);
                var list = new List<object>();
                for (var round = 0; round < 3; round++)
                {
                    foreach (var m in PersonMethods)
                    {
                        list.Add(m.Call(g));
                    }
                }

                return list;
            });

            Add("Seeded StarNameGenerator", "every method in turn on one generator, seed 2024", () =>
            {
                var g = new StarNameGenerator(2024);
                var list = new List<object>();
                for (var round = 0; round < 5; round++)
                {
                    foreach (var m in StarMethods)
                    {
                        list.Add(m.Call(g));
                    }
                }

                return list;
            });

            foreach (var seed in new[] { 7, 2026 })
            {
                var s = seed.ToString(CultureInfo.InvariantCulture);
                Add("Seeded large", "GenerateRandomFirstAndLastName x10000 seed " + s, () =>
                {
                    var g = new PersonNameGenerator(seed);
                    return Summary(Draw(10000, g.GenerateRandomFirstAndLastName).Cast<string>().ToList());
                });
                Add("Seeded large", "GenerateRandomPlaceName x10000 seed " + s, () =>
                {
                    var g = new PlaceNameGenerator(seed);
                    return Summary(Draw(10000, g.GenerateRandomPlaceName).Cast<string>().ToList());
                });
                Add("Seeded large", "GenerateRandomStarName x10000 seed " + s, () =>
                {
                    var g = new StarNameGenerator(seed);
                    return Summary(Draw(10000, g.GenerateRandomStarName).Cast<string>().ToList());
                });
                Add("Seeded large", "GenerateRandomCatalogStarName x10000 seed " + s, () =>
                {
                    var g = new StarNameGenerator(seed);
                    return Summary(Draw(10000, g.GenerateRandomCatalogStarName).Cast<string>().ToList());
                });
            }
        }

        private static void ConstructorCases()
        {
            foreach (var seed in new[] { 7, -7 })
            {
                var s = seed.ToString(CultureInfo.InvariantCulture);
                Add("Constructors", "PersonNameGenerator(new Random(seed)) equals PersonNameGenerator(seed), seed " + s, () =>
                {
                    var a = new PersonNameGenerator(new Random(seed));
                    var b = new PersonNameGenerator(seed);
                    return Draw(20, a.GenerateRandomFirstAndLastName).Cast<string>().SequenceEqual(Draw(20, b.GenerateRandomFirstAndLastName).Cast<string>());
                });
                Add("Constructors", "PlaceNameGenerator(new Random(seed)) equals PlaceNameGenerator(seed), seed " + s, () =>
                {
                    var a = new PlaceNameGenerator(new Random(seed));
                    var b = new PlaceNameGenerator(seed);
                    return Draw(20, a.GenerateRandomPlaceName).Cast<string>().SequenceEqual(Draw(20, b.GenerateRandomPlaceName).Cast<string>());
                });
                Add("Constructors", "StarNameGenerator(new Random(seed)) equals StarNameGenerator(seed), seed " + s, () =>
                {
                    var a = new StarNameGenerator(new Random(seed));
                    var b = new StarNameGenerator(seed);
                    return Draw(20, a.GenerateRandomStarName).Cast<string>().SequenceEqual(Draw(20, b.GenerateRandomStarName).Cast<string>());
                });
            }

            Add("Constructors", "PersonNameGenerator(null Random)", () => new PersonNameGenerator((Random)null).GenerateRandomLastName());
            Add("Constructors", "PlaceNameGenerator(null Random)", () => new PlaceNameGenerator((Random)null).GenerateRandomPlaceName());
            Add("Constructors", "StarNameGenerator(null Random)", () => new StarNameGenerator((Random)null).GenerateRandomStarName());
            Add("Constructors", "interfaces implemented", () => Strings(new[]
            {
                typeof(IPersonNameGenerator).IsAssignableFrom(typeof(PersonNameGenerator)).ToString(),
                typeof(IPlaceNameGenerator).IsAssignableFrom(typeof(PlaceNameGenerator)).ToString(),
                typeof(IStarNameGenerator).IsAssignableFrom(typeof(StarNameGenerator)).ToString(),
                typeof(BaseNameGenerator).IsAssignableFrom(typeof(PersonNameGenerator)).ToString(),
                typeof(BaseNameGenerator).IsAssignableFrom(typeof(PlaceNameGenerator)).ToString(),
                typeof(BaseNameGenerator).IsAssignableFrom(typeof(StarNameGenerator)).ToString(),
            }));
        }

        private static void MultipleCases()
        {
            foreach (var m in PersonMultiples)
            {
                MultipleCase("PersonNameGenerator." + m.Name, n => m.Call(new PersonNameGenerator(99), n));
            }

            MultipleCase("PlaceNameGenerator.GenerateMultiplePlaceNames", n => new PlaceNameGenerator(99).GenerateMultiplePlaceNames(n));
            MultipleCase("StarNameGenerator.GenerateMultipleStarNames", n => new StarNameGenerator(99).GenerateMultipleStarNames(n));

            Add("Multiple", "an interface call gives the same names, seed 99", () =>
            {
                IPersonNameGenerator person = new PersonNameGenerator(99);
                IPlaceNameGenerator place = new PlaceNameGenerator(99);
                IStarNameGenerator star = new StarNameGenerator(99);
                return Strings(person.GenerateMultipleFirstAndLastNames(3).Concat(place.GenerateMultiplePlaceNames(3)).Concat(star.GenerateMultipleStarNames(3))
                    .Concat(new[] { person.GenerateRandomFirstAndLastName(), place.GenerateRandomPlaceName(), star.GenerateRandomCatalogStarName(), star.GenerateRandomProperStarName() }));
            });
        }

        private static void MultipleCase(string name, Func<int, IEnumerable<string>> call)
        {
            foreach (var n in Counts)
            {
                Add("Multiple", name + " count " + n.ToString(CultureInfo.InvariantCulture), () =>
                {
                    var result = call(n);
                    var o = new JsonObject();
                    o.Add("type", TypeName(result.GetType()));
                    o.Add("names", Strings(result));
                    o.Add("enumeratesTheSameTwice", result.SequenceEqual(result));
                    return o;
                });
            }

            foreach (var n in BadCounts)
            {
                Add("Multiple", name + " count " + n.ToString(CultureInfo.InvariantCulture), () => Strings(call(n)));
            }

            Add("Multiple", name + " count 10000", () => Summary(call(10000)));
        }

        private static void ExtensionCases()
        {
            foreach (var m in RandomExtensions)
            {
                Add("Random extensions", m.Name + " x10 on one Random, seed 5", () =>
                {
                    var r = new Random(5);
                    return Draw(Draws, () => m.Call(r));
                });
                Add("Random extensions", m.Name + " on a null Random", () => m.Call(null));
            }

            foreach (var m in RandomMultipleExtensions)
            {
                foreach (var n in new[] { 0, 3 })
                {
                    Add("Random extensions", m.Name + " count " + n.ToString(CultureInfo.InvariantCulture) + " twice on one Random, seed 5", () =>
                    {
                        var r = new Random(5);
                        var first = m.Call(r, n);
                        var second = m.Call(r, n);
                        var o = new JsonObject();
                        o.Add("type", TypeName(first.GetType()));
                        o.Add("first", Strings(first));
                        o.Add("second", Strings(second));
                        return o;
                    });
                }

                foreach (var n in BadCounts)
                {
                    Add("Random extensions", m.Name + " count " + n.ToString(CultureInfo.InvariantCulture), () => Strings(m.Call(new Random(5), n)));
                }

                Add("Random extensions", m.Name + " on a null Random, count 3", () => Strings(m.Call(null, 3)));
                Add("Random extensions", m.Name + " on a null Random, count -1", () => Strings(m.Call(null, -1)));
            }
        }

        private static void SharedRandomCases()
        {
            Add("Shared Random", "person, place, star, extensions and the caller's own draws on one Random, seed 314", () =>
            {
                var r = new Random(314);
                var person = new PersonNameGenerator(r);
                var place = new PlaceNameGenerator(r);
                var star = new StarNameGenerator(r);
                var list = new List<object>();
                for (var i = 0; i < 5; i++)
                {
                    list.Add(person.GenerateRandomFirstAndLastName());
                    list.Add(place.GenerateRandomPlaceName());
                    list.Add(star.GenerateRandomStarName());
                    list.Add(star.GenerateRandomCatalogStarName());
                    list.Add(r.GenerateRandomLastName());
                    list.Add(r.Next(1000));
                }

                return list;
            });
        }

        // ---------- how the library consumes a Random ----------

        private static void RandomCallCases()
        {
            foreach (var m in PersonMethods)
            {
                RecordCase("PersonNameGenerator." + m.Name, r => m.Call(new PersonNameGenerator(r)));
            }

            foreach (var m in PlaceMethods)
            {
                RecordCase("PlaceNameGenerator." + m.Name, r => m.Call(new PlaceNameGenerator(r)));
            }

            foreach (var m in StarMethods)
            {
                RecordCase("StarNameGenerator." + m.Name, r => m.Call(new StarNameGenerator(r)));
            }

            foreach (var m in PersonMultiples)
            {
                RecordCase("PersonNameGenerator." + m.Name + " count 3", r => Strings(m.Call(new PersonNameGenerator(r), 3)));
            }

            RecordCase("PlaceNameGenerator.GenerateMultiplePlaceNames count 3", r => Strings(new PlaceNameGenerator(r).GenerateMultiplePlaceNames(3)));
            RecordCase("StarNameGenerator.GenerateMultipleStarNames count 3", r => Strings(new StarNameGenerator(r).GenerateMultipleStarNames(3)));

            foreach (var m in RandomExtensions)
            {
                RecordCase("Random." + m.Name, r => m.Call(r));
            }

            foreach (var m in RandomMultipleExtensions)
            {
                RecordCase("Random." + m.Name + " count 3", r => Strings(m.Call(r, 3)));
            }

            RecordCase("constructors only", r =>
            {
                GC.KeepAlive(new PersonNameGenerator(r));
                GC.KeepAlive(new PlaceNameGenerator(r));
                GC.KeepAlive(new StarNameGenerator(r));
                return "constructed";
            });
        }

        private static void RecordCase(string name, Func<Random, object> call)
        {
            Add("Random calls", name + ", seed 2024", () =>
            {
                var r = new RecordingRandom(2024);
                var o = new JsonObject();
                o.Add("result", Try(() => call(r)));
                o.Add("calls", r.Calls);
                return o;
            });
        }

        private static void ScriptedCases()
        {
            var bounds = new[]
            {
                new KeyValuePair<string, Func<int, int, int>>("first", ScriptedRandom.Min),
                new KeyValuePair<string, Func<int, int, int>>("last", ScriptedRandom.Last),
                new KeyValuePair<string, Func<int, int, int>>("max (out of range)", ScriptedRandom.Max),
                new KeyValuePair<string, Func<int, int, int>>("min - 1 (out of range)", ScriptedRandom.BelowMin),
            };

            foreach (var b in bounds)
            {
                Scripted("GenerateRandomMaleFirstName " + b.Key, r => new PersonNameGenerator(r).GenerateRandomMaleFirstName(), b.Value);
                Scripted("GenerateRandomFemaleFirstName " + b.Key, r => new PersonNameGenerator(r).GenerateRandomFemaleFirstName(), b.Value);
                Scripted("GenerateRandomLastName " + b.Key, r => new PersonNameGenerator(r).GenerateRandomLastName(), b.Value);
                Scripted("GenerateRandomPlaceName " + b.Key, r => new PlaceNameGenerator(r).GenerateRandomPlaceName(), b.Value);
                Scripted("GenerateRandomStarName " + b.Key, r => new StarNameGenerator(r).GenerateRandomStarName(), b.Value);
                Scripted("GenerateRandomProperStarName " + b.Key, r => new StarNameGenerator(r).GenerateRandomProperStarName(), b.Value);
                Scripted("GenerateRandomCatalogStarName " + b.Key, r => new StarNameGenerator(r).GenerateRandomCatalogStarName(), b.Value);
                Scripted("Random.GenerateRandomPlaceName " + b.Key, r => r.GenerateRandomPlaceName(), b.Value);
            }

            var proper = StarNameGenerator.ProperStarNames.Count;
            Scripted("GenerateRandomStarName at the last proper name", r => new StarNameGenerator(r).GenerateRandomStarName(), ScriptedRandom.Value(proper - 1));
            Scripted("GenerateRandomStarName at the first designation", r => new StarNameGenerator(r).GenerateRandomStarName(), ScriptedRandom.Value(proper));
            Scripted("GenerateRandomCatalogStarName at HD 225300", r => new StarNameGenerator(r).GenerateRandomCatalogStarName(), ScriptedRandom.Value(225299));
            Scripted("GenerateRandomCatalogStarName at the first HIP", r => new StarNameGenerator(r).GenerateRandomCatalogStarName(), ScriptedRandom.Value(225300));
            Scripted("GenerateRandomCatalogStarName at the second HIP", r => new StarNameGenerator(r).GenerateRandomCatalogStarName(), ScriptedRandom.Value(225301));

            foreach (var v in new[] { 0, 1, 2, -1 })
            {
                Scripted("GenerateRandomFirstName with the gender draw " + v.ToString(CultureInfo.InvariantCulture), r => new PersonNameGenerator(r).GenerateRandomFirstName(), ScriptedRandom.Value(v), ScriptedRandom.Min);
            }

            Scripted("GenerateRandomFirstAndLastName, all first", r => new PersonNameGenerator(r).GenerateRandomFirstAndLastName(), ScriptedRandom.Min, ScriptedRandom.Min, ScriptedRandom.Min);
            Scripted("GenerateRandomFirstAndLastName, all last", r => new PersonNameGenerator(r).GenerateRandomFirstAndLastName(), ScriptedRandom.Last, ScriptedRandom.Last, ScriptedRandom.Last);
            Scripted("GenerateRandomFemaleFirstAndLastName, all last", r => new PersonNameGenerator(r).GenerateRandomFemaleFirstAndLastName(), ScriptedRandom.Last, ScriptedRandom.Last);
            Scripted("GenerateRandomMaleFirstAndLastName, all first", r => new PersonNameGenerator(r).GenerateRandomMaleFirstAndLastName(), ScriptedRandom.Min, ScriptedRandom.Min);
            Scripted("GenerateMultipleLastNames count 2 when the script ends after one", r => Strings(new PersonNameGenerator(r).GenerateMultipleLastNames(2)), ScriptedRandom.Min);
        }

        private static void Scripted(string name, Func<Random, object> call, params Func<int, int, int>[] steps)
        {
            Add("Scripted Random", name, () =>
            {
                var r = new ScriptedRandom(steps);
                var o = new JsonObject();
                o.Add("result", Try(() => call(r)));
                o.Add("calls", r.Calls);
                return o;
            });
        }

        // ---------- default (unseeded) generators ----------

        private static void DefaultCases()
        {
            Add("Default generators", "PersonNameGenerator() names come from the lists", () =>
            {
                var g = new PersonNameGenerator();
                var first = new HashSet<string>(PersonNameGenerator.MaleFirstNames.Concat(PersonNameGenerator.FemaleFirstNames), StringComparer.Ordinal);
                var last = new HashSet<string>(PersonNameGenerator.LastNames, StringComparer.Ordinal);
                return Enumerable.Range(0, 200).Select(_ => g.GenerateRandomFirstAndLastName().Split(' ')).All(p => p.Length == 2 && first.Contains(p[0]) && last.Contains(p[1]));
            });
            Add("Default generators", "PlaceNameGenerator() names come from the list", () =>
            {
                var g = new PlaceNameGenerator();
                var set = new HashSet<string>(PlaceNameGenerator.PlaceNames, StringComparer.Ordinal);
                return Enumerable.Range(0, 200).All(_ => set.Contains(g.GenerateRandomPlaceName()));
            });
            Add("Default generators", "StarNameGenerator() names come from the lists", () =>
            {
                var g = new StarNameGenerator();
                var set = new HashSet<string>(StarNameGenerator.StarNames, StringComparer.Ordinal);
                var proper = new HashSet<string>(StarNameGenerator.ProperStarNames, StringComparer.Ordinal);
                return Enumerable.Range(0, 200).All(_ => set.Contains(g.GenerateRandomStarName()) && proper.Contains(g.GenerateRandomProperStarName()));
            });
            Add("Default generators", "catalogue names are HD 1 to HD 225300 or HIP numbers", () =>
            {
                var g = new StarNameGenerator();
                return Enumerable.Range(0, 500).Select(_ => g.GenerateRandomCatalogStarName()).All(n =>
                    (n.StartsWith("HD ", StringComparison.Ordinal) && int.TryParse(n.Substring(3), NumberStyles.None, CultureInfo.InvariantCulture, out var hd) && hd >= 1 && hd <= 225300)
                    || (n.StartsWith("HIP ", StringComparison.Ordinal) && int.TryParse(n.Substring(4), NumberStyles.None, CultureInfo.InvariantCulture, out var hip) && hip >= 1));
            });
            Add("Default generators", "two PersonNameGenerator() made back to back give different names", () =>
            {
                var a = new PersonNameGenerator();
                var b = new PersonNameGenerator();
                return !Draw(20, a.GenerateRandomFirstAndLastName).Cast<string>().SequenceEqual(Draw(20, b.GenerateRandomFirstAndLastName).Cast<string>());
            });
            Add("Default generators", "two PlaceNameGenerator() made back to back give different names", () =>
            {
                var a = new PlaceNameGenerator();
                var b = new PlaceNameGenerator();
                return !Draw(20, a.GenerateRandomPlaceName).Cast<string>().SequenceEqual(Draw(20, b.GenerateRandomPlaceName).Cast<string>());
            });
            Add("Default generators", "default generators on 8 threads at once give names from the lists", () =>
            {
                var set = new HashSet<string>(PlaceNameGenerator.PlaceNames, StringComparer.Ordinal);
                var ok = true;
                var threads = Enumerable.Range(0, 8).Select(_ => new Thread(() =>
                {
                    var g = new PlaceNameGenerator();
                    for (var i = 0; i < 2000; i++)
                    {
                        if (!set.Contains(g.GenerateRandomPlaceName()))
                        {
                            ok = false;
                        }
                    }
                })).ToList();
                threads.ForEach(t => t.Start());
                threads.ForEach(t => t.Join());
                return ok;
            });
            Add("Default generators", "the default Random's type", () => TypeName(new SubclassGenerator().Rand.GetType()));
            Add("Default generators", "two default generators share one Random", () => ReferenceEquals(new SubclassGenerator().Rand, new SubclassGenerator().Rand));
        }

        // ---------- a caller's subclass of BaseNameGenerator ----------

        private static void SubclassCases()
        {
            foreach (var name in new[] { "dist.male.first.stripped", "dist.female.first.stripped", "dist.all.last.stripped", "places2k.txt.stripped", "stars.proper.stripped", "stars.designations.stripped", "stars.hipgaps.stripped" })
            {
                Add("Subclass", "ReadResourceByLine " + name, () =>
                {
                    var lines = SubclassGenerator.Read(name);
                    var o = new JsonObject();
                    o.Add("count", lines.Length);
                    o.Add("sha256", Sha256(lines));
                    o.Add("first", Strings(lines.Take(2)));
                    return o;
                });
            }

            foreach (var name in new[] { "nope", "", null, "Resources.dist.male.first.stripped", "DIST.MALE.FIRST.STRIPPED", "../dist.male.first.stripped" })
            {
                Add("Subclass", "ReadResourceByLine " + (name == null ? "(null)" : "'" + name + "'"), () => SubclassGenerator.Read(name).Length);
            }

            Add("Subclass", "ReadResourceByLine returns a new array each call", () => !ReferenceEquals(SubclassGenerator.Read("stars.proper.stripped"), SubclassGenerator.Read("stars.proper.stripped")));
            Add("Subclass", "constructor with a null Random", () => new SubclassGenerator(null).Rand == null);
            Add("Subclass", "constructor keeps the caller's Random", () =>
            {
                var r = new Random(1);
                return ReferenceEquals(new SubclassGenerator(r).Rand, r);
            });
        }

        // ---------- cultures ----------

        private static void CultureCases()
        {
            foreach (var culture in new[] { "tr-TR", "fa-IR", "ar-SA", "hi-IN" })
            {
                Add("Culture", "seeded names under " + culture + ", seed 77", () =>
                {
                    var saved = Thread.CurrentThread.CurrentCulture;
                    try
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                        var star = new StarNameGenerator(77);
                        var person = new PersonNameGenerator(77);
                        return Strings(Draw(8, star.GenerateRandomCatalogStarName).Cast<string>().Concat(Draw(4, person.GenerateRandomFirstAndLastName).Cast<string>()));
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = saved;
                    }
                });
            }
        }

        // ---------- the obsolete build-time tools ----------

        private static void CensusListStripperCases()
        {
            var arabicIndic = new string(new[] { (char)0x0663, (char)0x0664, 'a' });
            var fullWidth = new string(new[] { (char)0xFF11, (char)0xFF12, 'b' });
            foreach (var input in new[] { "abc123", "", "no digits", "2020 Census 99", arabicIndic, fullWidth, "x" + (char)0x00B2 + "y" })
            {
                Add("CensusListStripper", "RemoveDigits " + string.Join(" ", input.Select(c => ((int)c).ToString("x4", CultureInfo.InvariantCulture))), () => CensusListStripper.RemoveDigits(input));
            }

            Add("CensusListStripper", "RemoveDigits null", () => CensusListStripper.RemoveDigits(null));

            var latin1 = Encoding.GetEncoding("iso-8859-1");
            var person = new[]
            {
                new KeyValuePair<string, byte[]>("three rows", Encoding.UTF8.GetBytes("JAMES          3.318  3.318      1\nJOHN           3.271  6.589      2\nMARY-ANN       0.100  6.689      3\n")),
                new KeyValuePair<string, byte[]>("CRLF rows", Encoding.UTF8.GetBytes("SMITH          1.006  1.006      1\r\nJOHNSON        0.810  1.816      2\r\n")),
                new KeyValuePair<string, byte[]>("one name, no statistics", Encoding.UTF8.GetBytes("o'NEIL")),
                new KeyValuePair<string, byte[]>("a blank row", Encoding.UTF8.GetBytes("JAMES 1\n\nJOHN 2\n")),
                new KeyValuePair<string, byte[]>("a leading space", Encoding.UTF8.GetBytes(" JAMES 1\n")),
                new KeyValuePair<string, byte[]>("Latin-1 bytes", latin1.GetBytes("MU" + (char)0x00D1 + "OZ          0.100  0.100      1\n")),
                new KeyValuePair<string, byte[]>("empty file", new byte[0]),
            };
            foreach (var p in person)
            {
                Add("CensusListStripper", "StripStatisticsFromPersonNameFile " + p.Key, () =>
                {
                    var input = Path2("person-in.txt");
                    var output = Path2("person-out.txt");
                    File.WriteAllBytes(input, p.Value);
                    if (File.Exists(output))
                    {
                        File.Delete(output);
                    }

                    new CensusListStripper().StripStatisticsFromPersonNameFile(input, output);
                    return ReadOutput(output);
                });
            }

            var place = new[]
            {
                new KeyValuePair<string, string>("a city", "AL0100124Abbeville city                                                        2987     1353    40341664       11430    15.575822     0.004413  31.566367 -85.251300"),
                new KeyValuePair<string, string>("zona urbana", "PR7200001Adjuntas zona urbana                                                   5239     2023     3374006           0     1.302709     0.000000  18.163470 -66.723157"),
                new KeyValuePair<string, string>("New York city", "NY3651000New York city                                                       8008278  3200912   785561207   429461513   303.306758   165.815627  40.714269 -74.007124"),
                new KeyValuePair<string, string>("a CDP whose name contains town", "CT0952000Newtown CDP                                                            1843      720     2000000           0     0.772000     0.000000  41.414000 -73.303000"),
                new KeyValuePair<string, string>("a village", "NY3600155Addison village                                                        1797      767     4136582       46271     1.597141     0.017865  42.104031 -77.232837"),
                new KeyValuePair<string, string>("a balance", "GA1304204Athens-Clarke County (balance)                                       101489    41633   307811470     2475590   118.846785     0.955829  33.951889 -83.365897"),
                new KeyValuePair<string, string>("state only", "XX"),
                new KeyValuePair<string, string>("one character", "A"),
                new KeyValuePair<string, string>("blank line", ""),
            };
            foreach (var p in place)
            {
                Add("CensusListStripper", "StripStatisticsFromPlaceNameFile " + p.Key, () =>
                {
                    var input = Path2("place-in.txt");
                    var output = Path2("place-out.txt");
                    File.WriteAllText(input, p.Value + "\n", new UTF8Encoding(false));
                    if (File.Exists(output))
                    {
                        File.Delete(output);
                    }

                    new CensusListStripper().StripStatisticsFromPlaceNameFile(input, output);
                    return ReadOutput(output);
                });
            }

            Add("CensusListStripper", "StripStatisticsFromPersonNameFile with a missing input", () =>
            {
                new CensusListStripper().StripStatisticsFromPersonNameFile(Path2("missing.txt"), Path2("out.txt"));
                return "no exception";
            });
            Add("CensusListStripper", "StripStatisticsFromPlaceNameFile with a missing input", () =>
            {
                new CensusListStripper().StripStatisticsFromPlaceNameFile(Path2("missing.txt"), Path2("out.txt"));
                return "no exception";
            });
            Add("CensusListStripper", "StripStatisticsFromPersonNameFile with a null path", () =>
            {
                new CensusListStripper().StripStatisticsFromPersonNameFile(null, Path2("out.txt"));
                return "no exception";
            });
        }

        private static void FileCompressorCases()
        {
            var texts = new[]
            {
                new KeyValuePair<string, string>("hello world", "hello world"),
                new KeyValuePair<string, string>("empty", ""),
                new KeyValuePair<string, string>("accents and a surrogate pair", "Bayam" + (char)0x00F3 + "n " + (char)0xD83D + (char)0xDE80),
                new KeyValuePair<string, string>("every last name", string.Join("\n", PersonNameGenerator.LastNames)),
            };
            foreach (var t in texts)
            {
                Add("FileCompressor", "round trip " + t.Key, () =>
                {
                    var path = Path2("compressed.bin");
                    var fc = new FileCompressor();
                    fc.CompressStringToFile(t.Value, path);
                    var bytes = File.ReadAllBytes(path);
                    var back = fc.DecompressFileToString(path);
                    var o = new JsonObject();
                    o.Add("roundTrips", back == t.Value);
                    o.Add("compressedBytes", bytes.Length);
                    o.Add("compressedSha256", Sha256(bytes));
                    return o;
                });
            }

            Add("FileCompressor", "CompressStringToFile null text", () =>
            {
                var path = Path2("null.bin");
                var fc = new FileCompressor();
                fc.CompressStringToFile(null, path);
                return fc.DecompressFileToString(path);
            });
            Add("FileCompressor", "DecompressFileToString of a stored deflate block", () =>
            {
                var path = Path2("stored.bin");
                File.WriteAllBytes(path, new byte[] { 0x01, 0x03, 0x00, 0xFC, 0xFF, (byte)'a', (byte)'b', (byte)'c' });
                return new FileCompressor().DecompressFileToString(path);
            });
            Add("FileCompressor", "DecompressFileToString of plain text", () =>
            {
                var path = Path2("plain.txt");
                File.WriteAllText(path, "not deflate data at all", new UTF8Encoding(false));
                return new FileCompressor().DecompressFileToString(path);
            });
            Add("FileCompressor", "DecompressFileToString of an empty file", () =>
            {
                var path = Path2("empty.bin");
                File.WriteAllBytes(path, new byte[0]);
                return new FileCompressor().DecompressFileToString(path);
            });
            Add("FileCompressor", "DecompressFileToString of a missing file", () => new FileCompressor().DecompressFileToString(Path2("missing.bin")));
            Add("FileCompressor", "CompressStringToFile into a missing folder", () =>
            {
                new FileCompressor().CompressStringToFile("x", Path.Combine(_work, "no-such-folder", "x.bin"));
                return "no exception";
            });
        }
    }
}
