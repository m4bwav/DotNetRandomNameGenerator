using System;
using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    public class PersonNameGenerator : BaseNameGenerator, IPersonNameGenerator
    {
        private const string MaleFile = "dist.male.first.stripped";
        private const string FemaleFile = "dist.female.first.stripped";
        private const string LastNameFile = "dist.all.last.stripped";
        private static string[] _maleFirstNames;
        private static string[] _femaleFirstNames;
        private static string[] _lastNames;

        public PersonNameGenerator()
        {
            InitNames();
        }

        public PersonNameGenerator(Random randGen) : base(randGen)
        {
            if (randGen == null) throw new ArgumentNullException(nameof(randGen));

            InitNames();
        }

        private bool RandomlyPickIfNameIsMale => RandGen.Next(0, 2) == 0;

        public string GenerateRandomFirstAndLastName()
        {
            return GenerateRandomFirstName() + ' ' + GenerateRandomLastName();
        }

        public string GenerateRandomLastName()
        {
            var index = RandGen.Next(0, _lastNames.Length);

            return _lastNames[index];
        }

        public string GenerateRandomFirstName()
        {
            return !RandomlyPickIfNameIsMale
                ? GenerateRandomFemaleFirstName()
                : GenerateRandomMaleFirstName();
        }

        public string GenerateRandomFemaleFirstName()
        {
            var index = RandGen.Next(0, _femaleFirstNames.Length);

            return _femaleFirstNames[index];
        }

        public string GenerateRandomMaleFirstName()
        {
            var index = RandGen.Next(0, _maleFirstNames.Length);

            return _maleFirstNames[index];
        }

        public IEnumerable<string> GenerateMultipleFirstAndLastNames(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var list = new List<string>();

            for (var index = 0; index < count; ++index)
                list.Add(GenerateRandomFirstAndLastName());

            return list;
        }

        public IEnumerable<string> GenerateMultipleLastNames(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var list = new List<string>();

            for (var index = 0; index < count; ++index)
                list.Add(GenerateRandomLastName());

            return list;
        }

        public IEnumerable<string> GenerateMultipleFemaleFirstAndLastNames(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var list = new List<string>();

            for (var index = 0; index < count; ++index)
                list.Add(GenerateRandomFemaleFirstAndLastName());

            return list;
        }

        public IEnumerable<string> GenerateMultipleMaleFirstAndLastNames(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var list = new List<string>();

            for (var index = 0; index < count; ++index)
                list.Add(GenerateRandomMaleFirstAndLastName());

            return list;
        }

        public IEnumerable<string> GenerateMultipleFemaleFirstNames(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var list = new List<string>();

            for (var index = 0; index < count; ++index)
                list.Add(GenerateRandomFemaleFirstName());

            return list;
        }

        public IEnumerable<string> GenerateMultipleMaleFirstNames(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            var list = new List<string>();

            for (var index = 0; index < count; ++index)
                list.Add(GenerateRandomMaleFirstName());

            return list;
        }

        public string GenerateRandomFemaleFirstAndLastName()
        {
            return GenerateRandomFemaleFirstName() + ' ' + GenerateRandomLastName();
        }

        public string GenerateRandomMaleFirstAndLastName()
        {
            return GenerateRandomMaleFirstName() + ' ' + GenerateRandomLastName();
        }

        private static void InitNames()
        {
            if (_maleFirstNames == null)
                _maleFirstNames = ReadResourceByLine(MaleFile);

            if (_femaleFirstNames == null)
                _femaleFirstNames = ReadResourceByLine(FemaleFile);

            if (_lastNames != null)
                return;

            _lastNames = ReadResourceByLine(LastNameFile);
        }
    }
}