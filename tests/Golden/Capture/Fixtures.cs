#nullable disable

// Fixtures for the golden capture: Random subclasses that record or script every call the library makes, and a
// BaseNameGenerator subclass that reaches its protected members the way a caller's subclass can.
// Touches only public and protected names of RandomNameGeneratorLibrary, so the golden test compiles it unchanged.

using System;
using System.Collections.Generic;
using System.Globalization;
using RandomNameGeneratorLibrary;

namespace GoldenCapture.Fixtures
{
    /// <summary>A seeded Random that logs every call and its answer, so a case pins how the library consumes it.</summary>
    public sealed class RecordingRandom : Random
    {
        public readonly List<object> Calls = new List<object>();

        public RecordingRandom(int seed) : base(seed)
        {
        }

        public override int Next()
        {
            var v = base.Next();
            Calls.Add("Next()=" + I(v));
            return v;
        }

        public override int Next(int maxValue)
        {
            var v = base.Next(maxValue);
            Calls.Add("Next(" + I(maxValue) + ")=" + I(v));
            return v;
        }

        public override int Next(int minValue, int maxValue)
        {
            var v = base.Next(minValue, maxValue);
            Calls.Add("Next(" + I(minValue) + "," + I(maxValue) + ")=" + I(v));
            return v;
        }

        public override double NextDouble()
        {
            Calls.Add("NextDouble()");
            return base.NextDouble();
        }

        public override void NextBytes(byte[] buffer)
        {
            Calls.Add("NextBytes(" + I(buffer == null ? -1 : buffer.Length) + ")");
            base.NextBytes(buffer);
        }

#if NET
        public override void NextBytes(Span<byte> buffer)
        {
            Calls.Add("NextBytes(Span " + I(buffer.Length) + ")");
            base.NextBytes(buffer);
        }

        public override long NextInt64()
        {
            Calls.Add("NextInt64()");
            return base.NextInt64();
        }

        public override long NextInt64(long maxValue)
        {
            Calls.Add("NextInt64(max)");
            return base.NextInt64(maxValue);
        }

        public override long NextInt64(long minValue, long maxValue)
        {
            Calls.Add("NextInt64(min,max)");
            return base.NextInt64(minValue, maxValue);
        }

        public override float NextSingle()
        {
            Calls.Add("NextSingle()");
            return base.NextSingle();
        }
#endif

        private static string I(int v)
        {
            return v.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>
    /// A Random whose Next(min, max) answers come from a script of steps, each given the bounds the library asked for.
    /// Lets a case pick the first or last entry of a list, or hand the library an answer outside its bounds.
    /// Any other member, or a call after the script ends, throws.
    /// </summary>
    public sealed class ScriptedRandom : Random
    {
        public readonly List<object> Calls = new List<object>();
        private readonly Func<int, int, int>[] _steps;
        private int _next;

        public ScriptedRandom(params Func<int, int, int>[] steps) : base(0)
        {
            _steps = steps;
        }

        public static Func<int, int, int> Min => (a, b) => a;

        public static Func<int, int, int> Last => (a, b) => b - 1;

        public static Func<int, int, int> Max => (a, b) => b;

        public static Func<int, int, int> BelowMin => (a, b) => a - 1;

        public static Func<int, int, int> Value(int value)
        {
            return (a, b) => value;
        }

        public override int Next(int minValue, int maxValue)
        {
            if (_next >= _steps.Length)
            {
                throw new InvalidOperationException("script exhausted");
            }

            var v = _steps[_next++](minValue, maxValue);
            Calls.Add("Next(" + minValue.ToString(CultureInfo.InvariantCulture) + "," + maxValue.ToString(CultureInfo.InvariantCulture) + ")=" + v.ToString(CultureInfo.InvariantCulture));
            return v;
        }

        public override int Next()
        {
            throw new NotSupportedException("ScriptedRandom.Next()");
        }

        public override int Next(int maxValue)
        {
            throw new NotSupportedException("ScriptedRandom.Next(max)");
        }

        public override double NextDouble()
        {
            throw new NotSupportedException("ScriptedRandom.NextDouble()");
        }

        protected override double Sample()
        {
            throw new NotSupportedException("ScriptedRandom.Sample()");
        }
    }

    /// <summary>A caller's subclass of the public abstract BaseNameGenerator, reaching its protected members.</summary>
    public sealed class SubclassGenerator : BaseNameGenerator
    {
        public SubclassGenerator()
        {
        }

        public SubclassGenerator(Random randGen) : base(randGen)
        {
        }

        public Random Rand => RandGen;

        public static string[] Read(string resourceFileName)
        {
            return ReadResourceByLine(resourceFileName);
        }
    }
}
