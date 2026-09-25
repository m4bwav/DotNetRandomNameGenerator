using System;
using System.Collections.Generic;
using System.Linq;
using RandomNameGeneratorLibrary;
using Xunit;

namespace RandomNameGeneratorUnitTests
{
    public class PersonNameGeneratorBehavior
    {
        [Fact]
        public void GetTheProperNumberOfRequestedNamesWithoutRepeats()
        {
            var result = new PersonNameGenerator(42).GenerateMultipleFirstAndLastNames(2).ToList();

            Assert.Equal(2, result.Count);
            Assert.NotEqual(result[0], result[1]);
        }

        [Fact]
        public void WhenAFirstAndLastNameAreGeneratedTogetherTheyShouldHaveASpaceBetweenThem()
        {
            var result = new PersonNameGenerator().GenerateRandomFirstAndLastName();

            Assert.NotNull(result);
            Assert.Contains(' ', result);
        }

        [Fact]
        public void WhenMultipleFemaleFirstAndLastNamesAreGeneratedTogetherTheyShouldHaveASpaceBetweenThem()
        {
            var result = new PersonNameGenerator().GenerateMultipleFemaleFirstAndLastNames(2).First();

            Assert.NotNull(result);
            Assert.Contains(' ', result);
        }

        [Fact]
        public void ShouldGenerateSameNameIfSameRandomGenerator()
        {
            var personNameGenerator1 = new PersonNameGenerator(new Random(42));
            var personNameGenerator2 = new PersonNameGenerator(new Random(42));

            Assert.Equal(personNameGenerator1.GenerateRandomFirstAndLastName(), personNameGenerator2.GenerateRandomFirstAndLastName());
        }

        [Fact]
        public void SeedConstructorMatchesSeededRandomConstructor()
        {
            var fromSeed = new PersonNameGenerator(7).GenerateMultipleFirstAndLastNames(5);
            var fromRandom = new PersonNameGenerator(new Random(7)).GenerateMultipleFirstAndLastNames(5);

            Assert.Equal(fromRandom, fromSeed);
        }

        [Fact]
        public void DefaultGeneratorsCreatedInATightLoopDoNotRepeatEachOther()
        {
            // GitHub issue #7: on .NET Framework new Random() is time-seeded, so twenty generators built in the
            // same tick used to produce twenty identical names.
            var names = new List<string>();
            for (var i = 0; i < 20; i++)
                names.Add(new PersonNameGenerator().GenerateRandomFirstAndLastName());

            Assert.True(names.Distinct().Count() > 1, "every default generator produced the same name");
        }

        [Fact]
        public void ZeroCountReturnsEmpty()
        {
            var generator = new PersonNameGenerator(1);

            Assert.Empty(generator.GenerateMultipleFirstAndLastNames(0));
            Assert.Empty(generator.GenerateMultipleLastNames(0));
            Assert.Empty(generator.GenerateMultipleFemaleFirstAndLastNames(0));
            Assert.Empty(generator.GenerateMultipleMaleFirstAndLastNames(0));
            Assert.Empty(generator.GenerateMultipleFemaleFirstNames(0));
            Assert.Empty(generator.GenerateMultipleMaleFirstNames(0));
        }

        [Fact]
        public void NegativeCountThrows()
        {
            var generator = new PersonNameGenerator(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GenerateMultipleFirstAndLastNames(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GenerateMultipleLastNames(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GenerateMultipleFemaleFirstAndLastNames(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GenerateMultipleMaleFirstAndLastNames(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GenerateMultipleFemaleFirstNames(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GenerateMultipleMaleFirstNames(-1));
        }

        [Fact]
        public void NullRandomThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new PersonNameGenerator(null!));
        }

        [Fact]
        public void MaleMethodsDrawFromTheMaleList()
        {
            var generator = new PersonNameGenerator(3);
            var male = new HashSet<string>(PersonNameGenerator.MaleFirstNames);

            Assert.All(generator.GenerateMultipleMaleFirstNames(200), name => Assert.Contains(name, male));
            Assert.All(generator.GenerateMultipleMaleFirstAndLastNames(200), name => Assert.Contains(FirstWord(name), male));
        }

        [Fact]
        public void FemaleMethodsDrawFromTheFemaleList()
        {
            var generator = new PersonNameGenerator(3);
            var female = new HashSet<string>(PersonNameGenerator.FemaleFirstNames);

            Assert.All(generator.GenerateMultipleFemaleFirstNames(200), name => Assert.Contains(name, female));
            Assert.All(generator.GenerateMultipleFemaleFirstAndLastNames(200), name => Assert.Contains(FirstWord(name), female));
        }

        [Fact]
        public void LastNamesDrawFromTheLastNameList()
        {
            var generator = new PersonNameGenerator(3);
            var last = new HashSet<string>(PersonNameGenerator.LastNames);

            Assert.All(generator.GenerateMultipleLastNames(200), name => Assert.Contains(name, last));
            Assert.All(generator.GenerateMultipleFirstAndLastNames(200), name => Assert.Contains(LastWord(name), last));
        }

        [Fact]
        public void EitherGenderFirstNameUsesBothLists()
        {
            var generator = new PersonNameGenerator(5);
            var male = new HashSet<string>(PersonNameGenerator.MaleFirstNames);
            var female = new HashSet<string>(PersonNameGenerator.FemaleFirstNames);
            var names = Enumerable.Range(0, 200).Select(_ => generator.GenerateRandomFirstName()).ToList();

            Assert.All(names, name => Assert.True(male.Contains(name) || female.Contains(name), name));
            Assert.Contains(names, name => male.Contains(name) && !female.Contains(name));
            Assert.Contains(names, name => female.Contains(name) && !male.Contains(name));
        }

        private static string FirstWord(string name) => name.Substring(0, name.IndexOf(' '));

        private static string LastWord(string name) => name.Substring(name.IndexOf(' ') + 1);
    }
}
