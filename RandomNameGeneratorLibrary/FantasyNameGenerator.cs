using System;
using System.Collections.Generic;

namespace RandomNameGeneratorLibrary
{
    /// <summary>
    /// Generates names for fantasy settings from small word grammars: places, landform places, factions,
    /// war-bands, trade surnames, bynames and keeper epithets. The word lists are written for this library and
    /// built into the code, so nothing is read from disk.
    /// </summary>
    /// <remarks>
    /// A generator created with the default constructor is safe to share between threads. A generator created
    /// around a caller-supplied <see cref="Random"/> is only as thread-safe as that instance, which for
    /// <see cref="Random"/> means not at all: use one generator per thread. The same seed gives the same names
    /// within a version of the library; a later version that adds words may give different ones.
    /// </remarks>
    public class FantasyNameGenerator : BaseNameGenerator, IFantasyNameGenerator
    {
        private static readonly string[] PlaceAdjectives =
        {
            "Broken", "Sunken", "Hollow", "Silent", "Burning", "Forgotten", "Crooked", "Gilded", "Shattered", "Weeping",
            "Iron", "Pale", "Crimson", "Drowned", "Lonely", "Thorned", "Ashen", "Golden", "Black", "White",
            "Old", "High", "Cold", "Bitter", "Starlit", "Rusted", "Sleeping", "Howling", "Veiled", "Last",
        };

        private static readonly string[] Structures =
        {
            "Tower", "Keep", "Abbey", "Hall", "Gate", "Bridge", "Temple", "Spire", "Citadel", "Mill",
            "Chapel", "Vault", "Crypt", "Fort", "Lighthouse", "Barrow", "Manor", "Shrine", "Library", "Well",
            "Market", "Watchpost", "Inn", "Forge", "Garden", "Throne", "Labyrinth", "Arch", "Halls", "Stair",
        };

        private static readonly string[] FeatureFirst =
        {
            "Ash", "Raven", "Salt", "Moon", "Thorn", "Frost", "Ember", "Bone", "Iron", "Mist",
            "Stone", "Wolf", "Gloam", "Copper", "Silver", "Storm", "Willow", "Hawk", "Cinder", "Amber",
        };

        private static readonly string[] FeatureSecond =
        {
            "Hollow", "Vale", "Fen", "Crag", "Hills", "Waste", "Reach", "Wood", "Shore", "Deep",
            "Moor", "Peaks", "Mere", "Glen", "Fields", "Falls", "March", "Downs", "Sands", "Isles",
        };

        private static readonly string[] HillPrefixes = { "Windy", "Green", "Bald", "Lark", "Bramble", "Stony", "Sheep", "Beacon", "Gorse", "Hare" };
        private static readonly string[] HillWords = { "Hill", "Tor", "Knoll", "Down", "Rise", "Ridge", "Barrow", "Brow", "Mound", "Heights" };
        private static readonly string[] RiverPrefixes = { "Swift", "Otter", "Silver", "Reed", "Mill", "Cold", "Alder", "Trout", "Willow", "Muddy" };
        private static readonly string[] RiverWords = { "Ford", "Bend", "Water", "Brook", "Run", "Rapids", "Crossing", "Falls", "Weir", "Banks" };
        private static readonly string[] ForestPrefixes = { "Oak", "Elder", "Shadow", "Bracken", "Moss", "Owl", "Hazel", "Deep", "Pine", "Antler" };
        private static readonly string[] ForestWords = { "Wood", "Grove", "Weald", "Thicket", "Holt", "Glade", "Copse", "Shaw", "Greenwood", "Tangle" };
        private static readonly string[] MarshPrefixes = { "Bog", "Black", "Rush", "Murk", "Heron", "Sedge", "Eel", "Toad", "Grey", "Sour" };
        private static readonly string[] MarshWords = { "Fen", "Mire", "Bog", "Marsh", "Slough", "Carr", "Moss", "Wallow", "Sump", "Flats" };
        private static readonly string[] CoastPrefixes = { "Saltmere", "Gull", "Wrack", "Gale", "Seal", "Driftwood", "Shell", "Tern", "Spray", "Pearl" };
        private static readonly string[] CoastWords = { "Strand", "Cove", "Bay", "Point", "Head", "Haven", "Sound", "Shore", "Cliffs", "Harbor" };
        private static readonly string[] MountainPrefixes = { "Eagle", "Frost", "Thunder", "Grim", "Snow", "Cloud", "Giant", "Iron", "Wind", "Goat" };
        private static readonly string[] MountainWords = { "Peak", "Crag", "Pass", "Spire", "Horn", "Summit", "Scarp", "Fang", "Tooth", "Col" };
        private static readonly string[] PlainPrefixes = { "Long", "Barley", "Gold", "Wide", "Thistle", "Wild", "Lark", "Dry", "Bison", "Sun" };
        private static readonly string[] PlainWords = { "Field", "Lea", "Moor", "Heath", "Steppe", "Meadow", "Plain", "Prairie", "Flat", "Acre" };

        private static readonly string[] TotemColors = { "Grey", "Red", "White", "Black", "Golden", "Silver", "Spotted", "Blue", "Pale", "Dun" };

        private static readonly string[] TotemAnimals =
        {
            "Heron", "Wolf", "Bear", "Elk", "Otter", "Raven", "Boar", "Hawk", "Badger", "Stag",
            "Lynx", "Salmon", "Owl", "Fox", "Bull", "Crane", "Viper", "Hare", "Ram", "Toad",
        };

        private static readonly string[] GroupNouns =
        {
            "Clan", "Tribe", "Band", "Kin", "Lodge", "Brotherhood", "Sisterhood", "Host", "Circle", "Company",
            "Fellowship", "Pack", "Council", "Hearth", "Sept",
        };

        private static readonly string[] WarVerbs =
        {
            "Split", "Break", "Crack", "Rend", "Gnaw", "Smash", "Bite", "Cleave", "Gouge", "Rip",
            "Crush", "Snap", "Bleed", "Burn", "Tear", "Grind", "Hack", "Pierce", "Gut", "Blind",
        };

        private static readonly string[] BodyParts =
        {
            "skull", "fang", "bone", "eye", "jaw", "hand", "spine", "throat", "heart", "tooth",
            "claw", "neck", "rib", "back", "knee", "horn", "tongue", "hide", "fist", "brow",
        };

        private static readonly string[] Occupations =
        {
            "Smith", "Taylor", "Baker", "Miller", "Cooper", "Fletcher", "Thatcher", "Mason", "Carter", "Turner",
            "Weaver", "Fisher", "Tanner", "Potter", "Brewer", "Butcher", "Chandler", "Sawyer", "Wright", "Glover",
            "Dyer", "Fuller", "Archer", "Bowyer", "Shepherd", "Hunter", "Porter", "Webster", "Barber", "Draper",
            "Mercer", "Slater", "Thresher", "Wainwright", "Carpenter", "Painter", "Spinner", "Skinner", "Cook", "Clark",
        };

        // Older or regional spellings, tried in order; the first whose pattern occurs after the first letter is
        // applied to its last occurrence.
        private static readonly string[][] SpellingRules =
        {
            new[] { "ith", "yth" },  // Smith to Smyth
            new[] { "ayl", "ail" },  // Taylor to Tailor
            new[] { "ark", "arke" }, // Clark to Clarke
            new[] { "ook", "ooke" }, // Cook to Cooke
            new[] { "ll", "l" },     // Miller to Miler
            new[] { "er", "ar" },    // Turner to Turnar
        };

        private static readonly string[] Bynames =
        {
            "Bold", "Fair", "Tall", "Short", "Red", "Grim", "Wise", "Young", "Elder", "Quiet",
            "Loud", "Swift", "Stout", "Lame", "Lucky", "Proud", "Gentle", "Fierce", "Black", "Fat",
            "Thin", "Merry", "Stern", "Clever", "Unready",
        };

        private static readonly string[] KeeperTraits =
        {
            "Honest", "Kind", "Watchful", "Patient", "Thrifty", "Generous", "Sober", "Cheerful", "Shrewd", "Careful",
            "Faithful", "Sharp-Eyed", "Tight-Fisted", "Soft-Spoken", "Steady",
        };

        private static readonly string[] KeeperDeeds =
        {
            "Lantern-Bearer", "Gate-Keeper", "Ale-Pourer", "Coin-Counter", "Bridge-Mender", "Well-Digger", "Wolf-Slayer",
            "Debt-Forgiver", "Map-Maker", "Bell-Ringer", "Road-Warden", "Bread-Giver", "Oath-Keeper", "Fire-Tender", "Stranger-Friend",
        };

        private readonly PersonNameGenerator _personNames;

        /// <summary>Creates a generator with a fresh, well-seeded random number generator.</summary>
        public FantasyNameGenerator()
        {
            _personNames = new PersonNameGenerator(RandGen);
        }

        /// <summary>Creates a generator that draws from <paramref name="randGen"/>.</summary>
        /// <param name="randGen">The random number generator to draw from. A seeded instance gives reproducible names.</param>
        /// <exception cref="ArgumentNullException"><paramref name="randGen"/> is null.</exception>
        public FantasyNameGenerator(Random randGen) : base(randGen)
        {
            _personNames = new PersonNameGenerator(RandGen);
        }

        /// <summary>Creates a generator that produces the same sequence of names for the same <paramref name="seed"/>.</summary>
        /// <param name="seed">The seed for the underlying <see cref="Random"/>.</param>
        public FantasyNameGenerator(int seed) : this(new Random(seed))
        {
        }

        /// <summary>The occupations <see cref="GenerateRandomTradeSurname"/> draws from, in their common spelling.</summary>
        public static IReadOnlyList<string> TradeSurnames => Occupations;

        /// <inheritdoc />
        public string GenerateRandomFantasyPlaceName()
        {
            var name = Pick(PlaceAdjectives) + " " + Pick(Structures);

            // About half the names get a feature: "of the Ash Hollow".
            if (RandGen.Next(0, 2) == 0)
                name += " of the " + Pick(FeatureFirst) + " " + Pick(FeatureSecond);

            return name;
        }

        /// <inheritdoc />
        public string GenerateRandomLandformPlaceName(Landform landform)
        {
            string[] prefixes;
            string[] words;

            switch (landform)
            {
                case Landform.Hill: prefixes = HillPrefixes; words = HillWords; break;
                case Landform.River: prefixes = RiverPrefixes; words = RiverWords; break;
                case Landform.Forest: prefixes = ForestPrefixes; words = ForestWords; break;
                case Landform.Marsh: prefixes = MarshPrefixes; words = MarshWords; break;
                case Landform.Coast: prefixes = CoastPrefixes; words = CoastWords; break;
                case Landform.Mountain: prefixes = MountainPrefixes; words = MountainWords; break;
                case Landform.Plain: prefixes = PlainPrefixes; words = PlainWords; break;
                default: throw new ArgumentOutOfRangeException(nameof(landform), landform, "Unknown landform.");
            }

            return Pick(prefixes) + " " + Pick(words);
        }

        /// <inheritdoc />
        public string GenerateRandomFactionName()
        {
            return "the " + Pick(TotemColors) + " " + Pick(TotemAnimals) + " " + Pick(GroupNouns);
        }

        /// <inheritdoc />
        public string GenerateRandomWarBandName()
        {
            return Pick(WarVerbs) + Pick(BodyParts);
        }

        /// <inheritdoc />
        public string GenerateRandomTradeSurname()
        {
            var surname = Pick(Occupations);

            // One name in three takes an older spelling when a rule applies to it.
            return RandGen.Next(0, 3) == 0 ? ApplySpellingVariant(surname) : surname;
        }

        /// <inheritdoc />
        public string GenerateRandomBynameName()
        {
            return _personNames.GenerateRandomFirstName() + " the " + Pick(Bynames);
        }

        /// <inheritdoc />
        public string GenerateRandomKeeperEpithet(string name)
        {
            if (name == null || name.Trim().Length == 0)
                throw new ArgumentException("The name cannot be null or white space.", nameof(name));

            var epithet = RandGen.Next(0, 2) == 0 ? Pick(KeeperTraits) : Pick(KeeperDeeds);

            return name.Trim() + " the " + epithet;
        }

        /// <inheritdoc />
        public IEnumerable<string> GenerateMultipleFantasyPlaceNames(int numberOfNames)
        {
            if (numberOfNames < 0) throw new ArgumentOutOfRangeException(nameof(numberOfNames), numberOfNames, "The number of names to generate cannot be negative.");

            var list = new List<string>(numberOfNames);

            for (var index = 0; index < numberOfNames; ++index)
                list.Add(GenerateRandomFantasyPlaceName());

            return list;
        }

        /// <summary>
        /// Rewrites <paramref name="surname"/> with the first spelling rule that applies to it, for example
        /// <c>Smith</c> to <c>Smyth</c> or <c>Taylor</c> to <c>Tailor</c>; a name no rule applies to comes back unchanged.
        /// </summary>
        /// <param name="surname">The surname to respell.</param>
        /// <returns>The respelled surname.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="surname"/> is null.</exception>
        public static string ApplySpellingVariant(string surname)
        {
            if (surname == null) throw new ArgumentNullException(nameof(surname));

            foreach (var rule in SpellingRules)
            {
                var at = surname.LastIndexOf(rule[0], StringComparison.Ordinal);
                if (at > 0)
                    return surname.Substring(0, at) + rule[1] + surname.Substring(at + rule[0].Length);
            }

            return surname;
        }

        private string Pick(string[] words)
        {
            return words[RandGen.Next(0, words.Length)];
        }
    }
}
